using FluentAssertions;
using FluentResults;
using OpenFTTH.CQRS;
using OpenFTTH.RouteNetwork.API.Model;
using OpenFTTH.RouteNetwork.API.Queries;
using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.IO;
using OpenFTTH.Schematic.Business.NodeSchematic;
using OpenFTTH.TestData;
using OpenFTTH.Util;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using OpenFTTH.UtilityGraphService.API.Queries;
using System.Linq;
using Xunit;

namespace OpenFTTH.Schematic.Tests.NodeSchematic
{
    public class NodeSchematicTests
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        private static TestConduitSpecifications _conduitSpecs;
        private static TestConduits _conduits;

        public NodeSchematicTests(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;

            _conduitSpecs = new TestConduitSpecifications(commandDispatcher, queryDispatcher).Run();
            _conduits = new TestConduits(commandDispatcher, queryDispatcher).Run();
        }

        [Fact]
        public async void TestDetachedMultiConduit_5x10_HH_1_to_HH_10()
        {
            var sutRouteNode = TestRouteNetwork.CC_1;

            // Query all route node interests
            var routeNetworkInterestQuery = new GetRouteNetworkDetails(new RouteNetworkElementIdList() { sutRouteNode })
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

            var spanEquipment = equipmentQueryResult.Value.SpanEquipment[_conduits.MultiConduit_5x10_HH_1_to_HH_10];

            // Create read model
            var readModel = new DetachedSpanEquipmentViewModel(sutRouteNode, spanEquipment, routeElementsQueryResult.Value.RouteNetworkElements, spanStructureSpecificationsQueryResult.Value);

            var builder = new DetachedSpanEquipmentBuilder(readModel);

            // Create the diagram
            Diagram diagram = new Diagram();

            builder.CreateDiagramObjects(diagram, 0, 0);

            // Assert
            diagram.DiagramObjects.Count(o => o.Style == "OuterConduitOrange").Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style.Contains("InnerConduit")).Should().Be(5);
            diagram.DiagramObjects.Count(o => o.Style == "VestTerminalLabel").Should().Be(5);
            diagram.DiagramObjects.Count(o => o.Label == "HH-1").Should().Be(5);
            diagram.DiagramObjects.Count(o => o.Style == "EastTerminalLabel").Should().Be(5);
            diagram.DiagramObjects.Count(o => o.Label == "HH-10").Should().Be(5);
            diagram.DiagramObjects.Count(o => o.IdentifiedObject != null && o.IdentifiedObject.RefClass == "SpanStructure").Should().Be(6);

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

        }
    }
}
