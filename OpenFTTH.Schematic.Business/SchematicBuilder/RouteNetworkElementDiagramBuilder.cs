using FluentResults;
using OpenFTTH.CQRS;
using OpenFTTH.RouteNetwork.API.Model;
using OpenFTTH.RouteNetwork.API.Queries;
using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Util;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using OpenFTTH.UtilityGraphService.API.Queries;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OpenFTTH.Schematic.Business.SchematicBuilder
{
    public class RouteNetworkElementDiagramBuilder
    {
        private readonly IQueryDispatcher _queryDispatcher;

        private readonly Diagram _diagram = new Diagram()
        {
            Margin = 0.01
        };

        private readonly double _spaceBetweenSections = 20;

        private Guid _routeNetworkElementId;
        private RouteNetworkElementRelatedData _data;

        public RouteNetworkElementDiagramBuilder(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;
        }

        public Task<Result<Diagram>> GetDiagram(Guid routeNetworkElementId)
        {
            _routeNetworkElementId = routeNetworkElementId;

            var fetchNeedeDataResult = FetchDataNeededToCreateDiagram(_queryDispatcher, routeNetworkElementId);

            if (fetchNeedeDataResult.IsFailed)
                return Task.FromResult(Result.Fail<Diagram>(fetchNeedeDataResult.Errors.First()));
            else
                _data = fetchNeedeDataResult.Value;

            // If no equipment found, just return an empty diagram
            if (_data.SpanEquipments.Count == 0)
            {
                return Task.FromResult(Result.Ok<Diagram>(new Diagram()));
            }

            AddDetachedSpanEquipmentsToDiagram();
          
            return Task.FromResult((Result.Ok<Diagram>(_diagram)));
        }

        private void AddDetachedSpanEquipmentsToDiagram()
        {
            var orderedInterestRelations = _data.InterestRelations.Values.OrderBy(i => i.RelationKind).Reverse();

            double yOffset = 0;

            foreach (var interestRelation in orderedInterestRelations)
            {
                var spanEquipment = _data.SpanEquipments.First(s => s.WalkOfInterest.Id == interestRelation.RefId);

                var readModel = new DetachedSpanEquipmentViewModel(_routeNetworkElementId, spanEquipment.Id, _data);

                var builder = new DetachedSpanEquipmentBuilder(readModel);

                var size = builder.CreateDiagramObjects(_diagram, 0, yOffset);

                yOffset += size.Height + _spaceBetweenSections;

            }
        }

        public static Result<RouteNetworkElementRelatedData> FetchDataNeededToCreateDiagram(IQueryDispatcher queryDispatcher, Guid routeNetworkElementId)
        {
            RouteNetworkElementRelatedData result = new RouteNetworkElementRelatedData();

            // Query all span equipment specifications
            result.SpanEquipmentSpecifications = queryDispatcher.HandleAsync<GetSpanEquipmentSpecifications, CSharpFunctionalExtensions.Result<LookupCollection<SpanEquipmentSpecification>>>(new GetSpanEquipmentSpecifications()).Result.Value;

            // Query all span structure specifications
            result.SpanStructureSpecifications = queryDispatcher.HandleAsync<GetSpanStructureSpecifications, CSharpFunctionalExtensions.Result<LookupCollection<SpanStructureSpecification>>>(new GetSpanStructureSpecifications()).Result.Value;

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
