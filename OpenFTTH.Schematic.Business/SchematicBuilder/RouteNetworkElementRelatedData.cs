using FluentResults;
using OpenFTTH.CQRS;
using OpenFTTH.RouteNetwork.API.Model;
using OpenFTTH.RouteNetwork.API.Queries;
using OpenFTTH.Util;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using OpenFTTH.UtilityGraphService.API.Queries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenFTTH.Schematic.Business.SchematicBuilder
{
    public class RouteNetworkElementRelatedData
    {
        public Guid RouteNetworkElementId { get; set; }
        public LookupCollection<SpanEquipmentSpecification> SpanEquipmentSpecifications { get; set; }
        public LookupCollection<SpanStructureSpecification> SpanStructureSpecifications { get; set; }
        public LookupCollection<NodeContainerSpecification> NodeContainerSpecifications { get; set; }
        public LookupCollection<RouteNetworkElement> RouteNetworkElements { get; set; }
        public LookupCollection<SpanEquipmentWithRelatedInfo> SpanEquipments { get; set; }
        public Dictionary<Guid, RouteNetworkElementInterestRelation> InterestRelations { get; set; }
        public NodeContainer NodeContainer { get; set; }
        public Guid NodeContainerRouteNetworkElementId { get; set; }

        public static Result<RouteNetworkElementRelatedData> FetchData(IQueryDispatcher queryDispatcher, Guid routeNetworkElementId)
        {
            RouteNetworkElementRelatedData result = new RouteNetworkElementRelatedData();

            result.RouteNetworkElementId = routeNetworkElementId;

            // TODO: All specifications should be cached and only re-queried if some spec is missing.

            // Query all span equipment specifications
            result.SpanEquipmentSpecifications = queryDispatcher.HandleAsync<GetSpanEquipmentSpecifications, CSharpFunctionalExtensions.Result<LookupCollection<SpanEquipmentSpecification>>>(new GetSpanEquipmentSpecifications()).Result.Value;

            // Query all span structure specifications
            result.SpanStructureSpecifications = queryDispatcher.HandleAsync<GetSpanStructureSpecifications, CSharpFunctionalExtensions.Result<LookupCollection<SpanStructureSpecification>>>(new GetSpanStructureSpecifications()).Result.Value;

            // Query all node container specifications
            result.NodeContainerSpecifications = queryDispatcher.HandleAsync<GetNodeContainerSpecifications, CSharpFunctionalExtensions.Result<LookupCollection<NodeContainerSpecification>>>(new GetNodeContainerSpecifications()).Result.Value;


            // Query all route node interests
            var routeNetworkInterestQuery = new GetRouteNetworkDetails(new RouteNetworkElementIdList() { routeNetworkElementId })
            {
                RelatedInterestFilter = RelatedInterestFilterOptions.ReferencesFromRouteElementAndInterestObjects
            };

            Result<GetRouteNetworkDetailsResult> interestsQueryResult = queryDispatcher.HandleAsync<GetRouteNetworkDetails, Result<GetRouteNetworkDetailsResult>>(routeNetworkInterestQuery).Result;

            if (interestsQueryResult.IsFailed)
                return Result.Fail(interestsQueryResult.Errors.First());

            result.InterestRelations = interestsQueryResult.Value.RouteNetworkElements.First().InterestRelations.ToDictionary(r => r.RefId);

            var interestIdList = new InterestIdList();
            interestIdList.AddRange(result.InterestRelations.Values.Select(r => r.RefId));

            // Only query for equipments if interests are returned from the route network query
            if (interestIdList.Count > 0)
            {
                // Query all the equipments related to the route network element
                var equipmentQueryResult = queryDispatcher.HandleAsync<GetEquipmentDetails, Result<GetEquipmentDetailsResult>>(new GetEquipmentDetails(interestIdList)).Result;

                if (equipmentQueryResult.IsFailed)
                    return Result.Fail(equipmentQueryResult.Errors.First());

                result.SpanEquipments = equipmentQueryResult.Value.SpanEquipment;

                if (equipmentQueryResult.Value.NodeContainers != null && equipmentQueryResult.Value.NodeContainers.Count > 0)
                {
                    result.NodeContainer = equipmentQueryResult.Value.NodeContainers.First();
                    result.NodeContainerRouteNetworkElementId = interestsQueryResult.Value.Interests[result.NodeContainer.InterestId].RouteNetworkElementRefs[0];
                }

                // Query all route network elements of all the equipments
                var routeNetworkElementsQuery = new GetRouteNetworkDetails(interestIdList);
                Result<GetRouteNetworkDetailsResult> routeElementsQueryResult = queryDispatcher.HandleAsync<GetRouteNetworkDetails, Result<GetRouteNetworkDetailsResult>>(routeNetworkElementsQuery).Result;

                result.RouteNetworkElements = routeElementsQueryResult.Value.RouteNetworkElements;
            }
            else
            {
                result.RouteNetworkElements = new LookupCollection<RouteNetworkElement>();
                result.SpanEquipments = new LookupCollection<SpanEquipmentWithRelatedInfo>();
            }

            return Result.Ok(result);
        }
    }
}
