using FluentResults;
using NetTopologySuite.Geometries;
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

        public RouteNetworkElementDiagramBuilder(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;
        }

        public async Task<Result<Diagram>> GetDiagram(Guid routeNodeElementId)
        {
            // Query all route node interests
            var routeNetworkInterestQuery = new GetRouteNetworkDetails(new RouteNetworkElementIdList() { routeNodeElementId })
            {
                RelatedInterestFilter = RelatedInterestFilterOptions.ReferencesFromRouteElementAndInterestObjects
            };

            Result<GetRouteNetworkDetailsResult> interestsQueryResult = await _queryDispatcher.HandleAsync<GetRouteNetworkDetails, Result<GetRouteNetworkDetailsResult>>(routeNetworkInterestQuery);

            var interestIdList = new InterestIdList();
            interestIdList.AddRange(interestsQueryResult.Value.RouteNetworkElements.First().InterestRelations.Select(r => r.RefId));

            // Query all equipments
            var equipmentQueryResult = await _queryDispatcher.HandleAsync<GetEquipmentDetails, Result<GetEquipmentDetailsResult>>(
               new GetEquipmentDetails(interestIdList)
            );

            // Query all span structure specifications
            var spanStructureSpecificationsQueryResult = await _queryDispatcher.HandleAsync<GetSpanStructureSpecifications, CSharpFunctionalExtensions.Result<LookupCollection<SpanStructureSpecification>>>(new GetSpanStructureSpecifications());

            // Query all route network elements of the interests
            var routeNetworkElementsQuery = new GetRouteNetworkDetails(interestIdList);
            Result<GetRouteNetworkDetailsResult> routeElementsQueryResult = await _queryDispatcher.HandleAsync<GetRouteNetworkDetails, Result<GetRouteNetworkDetailsResult>>(routeNetworkElementsQuery);

            var spanEquipment = equipmentQueryResult.Value.SpanEquipment.First();

            // Create read model
            var readModel = new DetachedSpanEquipmentViewModel(routeNodeElementId, spanEquipment, routeElementsQueryResult.Value.RouteNetworkElements, spanStructureSpecificationsQueryResult.Value);

            var builder = new DetachedSpanEquipmentBuilder(readModel);

            // Create the diagram
            Diagram diagram = new Diagram()
            {
                Margin = 0.01
            };

            builder.CreateDiagramObjects(diagram, 0, 0);

            return Result.Ok<Diagram>(diagram);
        }
    }
}
