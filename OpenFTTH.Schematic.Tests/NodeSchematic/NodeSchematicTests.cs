using FluentAssertions;
using FluentResults;
using OpenFTTH.CQRS;
using OpenFTTH.RouteNetwork.API.Model;
using OpenFTTH.RouteNetwork.API.Queries;
using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.IO;
using OpenFTTH.Schematic.Business.SchematicBuilder;
using OpenFTTH.TestData;
using OpenFTTH.Util;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using OpenFTTH.UtilityGraphService.API.Queries;
using System.Linq;
using Xunit;
using Xunit.Extensions.Ordering;

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

        [Fact, Order(1)]
        public async void TestDrawingSingleDetachedMultiConduit_5x10_HH_1_to_HH_10()
        {
            var sutRouteNode = TestRouteNetwork.CC_1;

            var data = RouteNetworkElementDiagramBuilder.FetchDataNeededToCreateDiagram(_queryDispatcher, sutRouteNode).Value;

            var spanEquipment = data.SpanEquipments[TestConduits.MultiConduit_5x10_HH_1_to_HH_10];

            // Create read model
            var readModel = new DetachedSpanEquipmentViewModel(sutRouteNode, spanEquipment.Id, data);

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
