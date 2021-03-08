using FluentAssertions;
using FluentResults;
using OpenFTTH.CQRS;
using OpenFTTH.EventSourcing;
using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.API.Queries;
using OpenFTTH.Schematic.Business.IO;
using OpenFTTH.Schematic.Business.SchematicBuilder;
using OpenFTTH.TestData;
using OpenFTTH.UtilityGraphService.API.Commands;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using OpenFTTH.UtilityGraphService.Business.Graph;
using System.Linq;
using Xunit;
using Xunit.Extensions.Ordering;

namespace OpenFTTH.Schematic.Tests.NodeSchematic
{
    [Order(2)]
    public class NodeSchematicTests
    {
        private readonly IEventStore _eventStore;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        private static TestSpecifications _specs;
        private static TestUtilityNetwork _utilityNetwork;

        public NodeSchematicTests(IEventStore eventStore, ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _eventStore = eventStore;
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;

            _specs = new TestSpecifications(commandDispatcher, queryDispatcher).Run();
            _utilityNetwork = new TestUtilityNetwork(commandDispatcher, queryDispatcher).Run();
        }

        [Fact, Order(1)]
        public void TestDrawingSingleDetachedMultiConduit_5x10_HH_1_to_HH_10()
        {
            var sutRouteNode = TestRouteNetwork.CC_1;

            var data = RouteNetworkElementRelatedData.FetchData(_queryDispatcher, sutRouteNode).Value;

            var spanEquipment = data.SpanEquipments[TestUtilityNetwork.MultiConduit_5x10_HH_1_to_HH_10];

            // Create read model
            var readModel = new SpanEquipmentViewModel(sutRouteNode, spanEquipment.Id, data);

            var builder = new DetachedSpanEquipmentBuilder(readModel);

            // Create the diagram
            Diagram diagram = new Diagram();

            builder.CreateDiagramObjects(diagram, 0, 0);

            // Assert
            diagram.DiagramObjects.Count(o => o.Style == "OuterConduitOrange").Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style.Contains("InnerConduit")).Should().Be(5);
            diagram.DiagramObjects.Count(o => o.Style == "WestTerminalLabel").Should().Be(5);
            diagram.DiagramObjects.Count(o => o.Label == "HH-1").Should().Be(5);
            diagram.DiagramObjects.Count(o => o.Style == "EastTerminalLabel").Should().Be(5);
            diagram.DiagramObjects.Count(o => o.Label == "HH-10").Should().Be(5);
            diagram.DiagramObjects.Count(o => o.IdentifiedObject != null && o.IdentifiedObject.RefClass == "SpanSegment").Should().Be(16);

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");
        }

        [Fact, Order(10)]
        public async void TestAffixConduitInCC_1()
        {
            // Affix 5x10 to west side
            var conduit1Id = TestUtilityNetwork.MultiConduit_5x10_HH_1_to_HH_10;

            var conduit1 = _eventStore.Projections.Get<UtilityGraphProjection>().SpanEquipments[conduit1Id];

            var conduit1AffixCommand = new AffixSpanEquipmentToNodeContainer(
                spanSegmentId: conduit1.SpanStructures[0].SpanSegments[0].Id,
                nodeContainerId: TestUtilityNetwork.NodeContainer_CC_1,
                nodeContainerIngoingSide: NodeContainerSideEnum.Vest
            );

            var conduit1AffixResult = await _commandDispatcher.HandleAsync<AffixSpanEquipmentToNodeContainer, Result>(conduit1AffixCommand);

            // Affix 3x10 to north side
            var conduit2Id = TestUtilityNetwork.MultiConduit_3x10_CC_1_to_SP_1;

            var conduit2 = _eventStore.Projections.Get<UtilityGraphProjection>().SpanEquipments[conduit2Id];

            var conduit2AffixCommand = new AffixSpanEquipmentToNodeContainer(
                spanSegmentId: conduit2.SpanStructures[0].SpanSegments[0].Id,
                nodeContainerId: TestUtilityNetwork.NodeContainer_CC_1,
                nodeContainerIngoingSide: NodeContainerSideEnum.North
            );

            var conduit2AffixResult = await _commandDispatcher.HandleAsync<AffixSpanEquipmentToNodeContainer, Result>(conduit2AffixCommand);

            // Affix flex conduit to south side
            var conduit3Id = TestUtilityNetwork.FlexConduit_40_Red_CC_1_to_SP_1;

            var conduit3 = _eventStore.Projections.Get<UtilityGraphProjection>().SpanEquipments[conduit3Id];

            var conduit3AffixCommand = new AffixSpanEquipmentToNodeContainer(
                spanSegmentId: conduit3.SpanStructures[0].SpanSegments[0].Id,
                nodeContainerId: TestUtilityNetwork.NodeContainer_CC_1,
                nodeContainerIngoingSide: NodeContainerSideEnum.South
            );

            var conduit3AffixResult = await _commandDispatcher.HandleAsync<AffixSpanEquipmentToNodeContainer, Result>(conduit3AffixCommand);



            // Act
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(TestRouteNetwork.CC_1));

            // Assert
            getDiagramQueryResult.IsSuccess.Should().BeTrue();
            var diagram = getDiagramQueryResult.Value.Diagram;

            diagram.DiagramObjects.Count(o => o.Style == "NodeContainer").Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "NodeContainerSideWest").Should().Be(1);

            diagram.DiagramObjects.Count(o => o.Style == "WestTerminalLabel" && o.IdentifiedObject.RefId == conduit1.SpanStructures[1].SpanSegments[0].Id).Should().Be(1);

            diagram.DiagramObjects.Count(o => o.Style == "NorthTerminalLabel" && o.IdentifiedObject.RefId == conduit2.SpanStructures[1].SpanSegments[0].Id).Should().Be(1);


            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(getDiagramQueryResult.Value.Diagram).Export("c:/temp/diagram/test.geojson");

        }
    }
}
