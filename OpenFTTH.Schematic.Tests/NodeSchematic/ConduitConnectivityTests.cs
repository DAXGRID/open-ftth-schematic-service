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
using System;
using System.Linq;
using Xunit;
using Xunit.Extensions.Ordering;

namespace OpenFTTH.Schematic.Tests.NodeSchematic
{
    [Order(3)]
    public class ConduitConnectivityTests
    {
        private readonly IEventStore _eventStore;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        private static TestUtilityNetwork _utilityNetwork;

        public ConduitConnectivityTests(IEventStore eventStore, ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _eventStore = eventStore;
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;

            new TestSpecifications(commandDispatcher, queryDispatcher).Run();
            _utilityNetwork = new TestUtilityNetwork(commandDispatcher, queryDispatcher).Run();
        }

        [Fact, Order(1)]
        public async void TestDrawingSpanEquipmentWithSegmentsCut()
        {
            var sutRouteNetworkElement = TestRouteNetwork.CC_1;
            var sutSpanEquipment = TestUtilityNetwork.MultiConduit_5x10_HH_1_to_HH_10;

            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            // Act
            utilityNetwork.TryGetEquipment<SpanEquipment>(sutSpanEquipment, out var spanEquipment);

            // Cut segments in structure 1 (the outer conduit and second inner conduit)
            var cutCmd = new CutSpanSegmentsAtRouteNode(
                routeNodeId: TestRouteNetwork.CC_1,
                spanSegmentsToCut: new Guid[] {
                    spanEquipment.SpanStructures[3].SpanSegments[0].Id
                }
            );

            var cutResult = await _commandDispatcher.HandleAsync<CutSpanSegmentsAtRouteNode, Result>(cutCmd);

            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));
            getDiagramQueryResult.IsSuccess.Should().BeTrue();

            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            cutResult.IsSuccess.Should().BeTrue();

            // Assert that north attached 3x10 has correct labels and id's
            utilityNetwork.TryGetEquipment<SpanEquipment>(TestUtilityNetwork.MultiConduit_3x10_CC_1_to_SP_1, out var cc1tosp1conduit);
            diagram.DiagramObjects.Count(o => o.Style == "OuterConduitOrange" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == cc1tosp1conduit.SpanStructures[0].SpanSegments[0].Id).Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "InnerConduitBlue" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == cc1tosp1conduit.SpanStructures[1].SpanSegments[0].Id).Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "InnerConduitYellow" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == cc1tosp1conduit.SpanStructures[2].SpanSegments[0].Id).Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "InnerConduitWhite" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == cc1tosp1conduit.SpanStructures[3].SpanSegments[0].Id).Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "SouthTerminalLabel" && o.Label == "SP-1" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == cc1tosp1conduit.SpanStructures[1].SpanSegments[0].Id).Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "SouthTerminalLabel" && o.Label == "SP-1" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == cc1tosp1conduit.SpanStructures[2].SpanSegments[0].Id).Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "SouthTerminalLabel" && o.Label == "SP-1" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == cc1tosp1conduit.SpanStructures[3].SpanSegments[0].Id).Should().Be(1);

            // Assert that 5x10 passing through node container has as correct labels and id's
            utilityNetwork.TryGetEquipment<SpanEquipment>(TestUtilityNetwork.MultiConduit_5x10_HH_1_to_HH_10, out var hh11tohh10conduit);
            diagram.DiagramObjects.Count(o => o.Style == "OuterConduitOrange" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == hh11tohh10conduit.SpanStructures[0].SpanSegments[0].Id).Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "OuterConduitOrange" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == hh11tohh10conduit.SpanStructures[0].SpanSegments[1].Id).Should().Be(1);

            diagram.DiagramObjects.Count(o => o.Style == "InnerConduitBlue" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == hh11tohh10conduit.SpanStructures[1].SpanSegments[0].Id).Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "WestTerminalLabel" && o.Label == "HH-1" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == hh11tohh10conduit.SpanStructures[1].SpanSegments[0].Id).Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "EastTerminalLabel" && o.Label == "HH-10" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == hh11tohh10conduit.SpanStructures[1].SpanSegments[0].Id).Should().Be(1);

            diagram.DiagramObjects.Count(o => o.Style == "InnerConduitYellow" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == hh11tohh10conduit.SpanStructures[2].SpanSegments[0].Id).Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "InnerConduitYellow" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == hh11tohh10conduit.SpanStructures[2].SpanSegments[1].Id).Should().Be(2);
            diagram.DiagramObjects.Count(o => o.Style == "WestTerminalLabel" && o.Label == "HH-1" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == hh11tohh10conduit.SpanStructures[2].SpanSegments[0].Id).Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "EastTerminalLabel" && o.Label == "HH-10" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == hh11tohh10conduit.SpanStructures[2].SpanSegments[1].Id).Should().Be(1);

            // Assert that 3x10 starting in CC and ending in HH 11 has as correct labels and id's
            utilityNetwork.TryGetEquipment<SpanEquipment>(TestUtilityNetwork.MultiConduit_3x10_CC_1_to_HH_11, out var cc1tohh1conduit);
            diagram.DiagramObjects.Count(o => o.Style == "WestTerminalLabel" && o.Label == "HH-11" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == cc1tohh1conduit.SpanStructures[1].SpanSegments[0].Id).Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "WestTerminalLabel" && o.Label == "HH-11" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == cc1tohh1conduit.SpanStructures[2].SpanSegments[0].Id).Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "WestTerminalLabel" && o.Label == "HH-11" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == cc1tohh1conduit.SpanStructures[3].SpanSegments[0].Id).Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "InnerConduitBlue" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == cc1tohh1conduit.SpanStructures[1].SpanSegments[0].Id).Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "InnerConduitYellow" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == cc1tohh1conduit.SpanStructures[2].SpanSegments[0].Id).Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "InnerConduitWhite" && o.IdentifiedObject.RefClass == "SpanSegment" && o.IdentifiedObject.RefId == cc1tohh1conduit.SpanStructures[3].SpanSegments[0].Id).Should().Be(1);

        }

        [Fact, Order(2)]
        public async void TestConnectWestSegmentToNorthSegmentInCC_1()
        {
            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            var sutRouteNetworkElement = TestRouteNetwork.CC_1;
            var sutSpanEquipment = TestUtilityNetwork.MultiConduit_5x10_HH_1_to_HH_10;

            var sutConnectFromSpanEquipment = TestUtilityNetwork.MultiConduit_5x10_HH_1_to_HH_10;
            var sutConnectToSpanEquipment = TestUtilityNetwork.MultiConduit_3x10_CC_1_to_SP_1;

            utilityNetwork.TryGetEquipment<SpanEquipment>(sutConnectFromSpanEquipment, out var sutFromSpanEquipment);
            utilityNetwork.TryGetEquipment<SpanEquipment>(sutConnectToSpanEquipment, out var sutToSpanEquipment);

            // Connect inner conduit 2 in 5x10 with inner conduit 3 in 3x10
            var connectCmd = new ConnectSpanSegmentsAtRouteNode(
                routeNodeId: TestRouteNetwork.CC_1,
                spanSegmentsToConnect: new Guid[] {
                    sutFromSpanEquipment.SpanStructures[2].SpanSegments[0].Id,
                    sutToSpanEquipment.SpanStructures[3].SpanSegments[0].Id
                }
            );

            var connectResult = await _commandDispatcher.HandleAsync<ConnectSpanSegmentsAtRouteNode, Result>(connectCmd);

            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));
            getDiagramQueryResult.IsSuccess.Should().BeTrue();

            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            connectResult.IsSuccess.Should().BeTrue();

        }

        [Fact, Order(10)]
        public async void TestSpanEquipmentDrawingPositionAccordingToOrder()
        {
            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            var sutRouteNetworkElementId = TestRouteNetwork.CC_1;
            var sutSpanEquipmentId1 = TestUtilityNetwork.MultiConduit_5x10_HH_1_to_HH_10;
            var sutSpanEquipmentId2 = TestUtilityNetwork.MultiConduit_3x10_CC_1_to_SP_1;
            var sutSpanEquipmentId3 = TestUtilityNetwork.MultiConduit_10x10_HH_1_to_HH_10;

            utilityNetwork.TryGetEquipment<SpanEquipment>(sutSpanEquipmentId1, out var sutWestAttachedSpanEquipment);
            utilityNetwork.TryGetEquipment<SpanEquipment>(sutSpanEquipmentId2, out var sutNorthAttachedSpanEquipment);
            utilityNetwork.TryGetEquipment<SpanEquipment>(sutSpanEquipmentId3, out var sutPassThroughSpanEquipment);

            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElementId));
            getDiagramQueryResult.IsSuccess.Should().BeTrue();

            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert

            // In west side, the span equipment must be drawed from top and down 1,2,3,4..
            var westSegment1MaxY = diagram.DiagramObjects.Find(d => d.IdentifiedObject != null && d.IdentifiedObject.RefId == sutWestAttachedSpanEquipment.SpanStructures[2].SpanSegments[0].Id).Geometry.EnvelopeInternal.MaxY;
            var westSegment2MaxY = diagram.DiagramObjects.Find(d => d.IdentifiedObject != null && d.IdentifiedObject.RefId == sutWestAttachedSpanEquipment.SpanStructures[3].SpanSegments[0].Id).Geometry.EnvelopeInternal.MaxY;
            westSegment1MaxY.Should().BeGreaterThan(westSegment2MaxY);

            // In south side, span equipment must be drawed from left to right 1,2,3,4..
            var northSegment1MaxX = diagram.DiagramObjects.Find(d => d.Style == "SouthTerminalLabel" && d.IdentifiedObject != null && d.IdentifiedObject.RefId == sutNorthAttachedSpanEquipment.SpanStructures[2].SpanSegments[0].Id).Geometry.EnvelopeInternal.MaxX;
            var northsegment2MaxX = diagram.DiagramObjects.Find(d => d.Style == "SouthTerminalLabel" && d.IdentifiedObject != null && d.IdentifiedObject.RefId == sutNorthAttachedSpanEquipment.SpanStructures[3].SpanSegments[0].Id).Geometry.EnvelopeInternal.MaxX;
            northsegment2MaxX.Should().BeGreaterThan(northSegment1MaxX);

            // In stand alone span equipment must be drawed from top and down 1,2,3,4..
            var standAloneSegment1MaxY = diagram.DiagramObjects.Find(d => d.IdentifiedObject != null && d.IdentifiedObject.RefId == sutPassThroughSpanEquipment.SpanStructures[2].SpanSegments[0].Id).Geometry.EnvelopeInternal.MaxY;
            var standAloneSegmentt2MaxY = diagram.DiagramObjects.Find(d => d.IdentifiedObject != null && d.IdentifiedObject.RefId == sutPassThroughSpanEquipment.SpanStructures[3].SpanSegments[0].Id).Geometry.EnvelopeInternal.MaxY;
            standAloneSegment1MaxY.Should().BeGreaterThan(standAloneSegmentt2MaxY);
        }

        [Fact, Order(50)]
        public async void TestHH_2()
        {
            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            var sutRouteNetworkElement = TestRouteNetwork.HH_2;
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));
            getDiagramQueryResult.IsSuccess.Should().BeTrue();

            var diagram = getDiagramQueryResult.Value.Diagram;

            // Assert
            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");
        }

    }
}
