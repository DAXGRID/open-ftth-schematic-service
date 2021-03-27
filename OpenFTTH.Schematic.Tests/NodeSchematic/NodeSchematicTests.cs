using FluentAssertions;
using FluentResults;
using OpenFTTH.CQRS;
using OpenFTTH.EventSourcing;
using OpenFTTH.RouteNetwork.API.Commands;
using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.API.Queries;
using OpenFTTH.Schematic.Business.IO;
using OpenFTTH.Schematic.Business.SchematicBuilder;
using OpenFTTH.TestData;
using OpenFTTH.UtilityGraphService.API.Commands;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using OpenFTTH.UtilityGraphService.API.Queries;
using OpenFTTH.UtilityGraphService.Business.Graph;
using System;
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

        private static TestUtilityNetwork _utilityNetwork;

        public NodeSchematicTests(IEventStore eventStore, ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _eventStore = eventStore;
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;

            new TestSpecifications(commandDispatcher, queryDispatcher).Run();
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

        [Fact, Order(2)]
        public async void TestAffixConduitInCC_1()
        {
            // Affix 5x10 to west side
            var conduit1Id = TestUtilityNetwork.MultiConduit_5x10_HH_1_to_HH_10;

            var conduit1 = _eventStore.Projections.Get<UtilityNetworkProjection>().SpanEquipments[conduit1Id];

            var conduit1AffixCommand = new AffixSpanEquipmentToNodeContainer(
                spanEquipmentOrSegmentId: conduit1.SpanStructures[0].SpanSegments[0].Id,
                nodeContainerId: TestUtilityNetwork.NodeContainer_CC_1,
                nodeContainerIngoingSide: NodeContainerSideEnum.West
            );

            var conduit1AffixResult = await _commandDispatcher.HandleAsync<AffixSpanEquipmentToNodeContainer, Result>(conduit1AffixCommand);

            // Affix 3x10 to north side
            var conduit2Id = TestUtilityNetwork.MultiConduit_3x10_CC_1_to_SP_1;

            var conduit2 = _eventStore.Projections.Get<UtilityNetworkProjection>().SpanEquipments[conduit2Id];

            var conduit2AffixCommand = new AffixSpanEquipmentToNodeContainer(
                spanEquipmentOrSegmentId: conduit2.SpanStructures[0].SpanSegments[0].Id,
                nodeContainerId: TestUtilityNetwork.NodeContainer_CC_1,
                nodeContainerIngoingSide: NodeContainerSideEnum.North
            );

            var conduit2AffixResult = await _commandDispatcher.HandleAsync<AffixSpanEquipmentToNodeContainer, Result>(conduit2AffixCommand);

            // Affix flex conduit to south side
            var conduit3Id = TestUtilityNetwork.FlexConduit_40_Red_CC_1_to_SP_1;

            var conduit3 = _eventStore.Projections.Get<UtilityNetworkProjection>().SpanEquipments[conduit3Id];

            var conduit3AffixCommand = new AffixSpanEquipmentToNodeContainer(
                spanEquipmentOrSegmentId: conduit3.SpanStructures[0].SpanSegments[0].Id,
                nodeContainerId: TestUtilityNetwork.NodeContainer_CC_1,
                nodeContainerIngoingSide: NodeContainerSideEnum.South
            );

            var conduit3AffixResult = await _commandDispatcher.HandleAsync<AffixSpanEquipmentToNodeContainer, Result>(conduit3AffixCommand);

            // Act
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(TestRouteNetwork.CC_1));

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(getDiagramQueryResult.Value.Diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            getDiagramQueryResult.IsSuccess.Should().BeTrue();
            var diagram = getDiagramQueryResult.Value.Diagram;

            diagram.DiagramObjects.Count(o => o.Style == "NodeContainer").Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "NodeContainerSideWest").Should().Be(1);

            diagram.DiagramObjects.Count(o => o.Style == "WestTerminalLabel" && o.IdentifiedObject.RefId == conduit1.SpanStructures[1].SpanSegments[0].Id).Should().Be(1);

            diagram.DiagramObjects.Count(o => o.Style == "NorthTerminalLabel" && o.IdentifiedObject.RefId == conduit2.SpanStructures[1].SpanSegments[0].Id).Should().Be(1);

            // Only one terminal connection should be shown in conduit 1 that is affixed to the node container
            diagram.DiagramObjects.Count(o => o.Style == "OuterConduitOrange" && o.IdentifiedObject.RefId == conduit1.SpanStructures[0].SpanSegments[0].Id).Should().Be(1);

            diagram.DiagramObjects.Any(d => d.DrawingOrder == 0).Should().BeFalse();
        }

        [Fact, Order(2)]
        public async void CutPassThroughConduitInCC1()
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
                    spanEquipment.SpanStructures[0].SpanSegments[0].Id,
                    spanEquipment.SpanStructures[2].SpanSegments[0].Id
                }
            );

            var cutResult = await _commandDispatcher.HandleAsync<CutSpanSegmentsAtRouteNode, Result>(cutCmd);

            cutResult.IsSuccess.Should().BeTrue();
        }


        [Fact, Order(3)]
        public async void TestAddAdditionalInnerConduitsToPassThroughConduitInJ_1()
        {
            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            var sutSpanEquipmentId = TestUtilityNetwork.FlexConduit_40_Red_SDU_1_to_SDU_2;

            utilityNetwork.TryGetEquipment<SpanEquipment>(sutSpanEquipmentId, out var sutSpanEquipment);

            // Add two inner conduits
            var addStructure = new PlaceAdditionalStructuresInSpanEquipment(
                spanEquipmentId: sutSpanEquipmentId,
                structureSpecificationIds: new Guid[] { TestSpecifications.Ø10_Red, TestSpecifications.Ø10_Violet }
            );

            var addStructureResult = await _commandDispatcher.HandleAsync<PlaceAdditionalStructuresInSpanEquipment, Result>(addStructure);

            // Add two more inner conduits
            var addStructure2 = new PlaceAdditionalStructuresInSpanEquipment(
                spanEquipmentId: sutSpanEquipmentId,
                structureSpecificationIds: new Guid[] { TestSpecifications.Ø10_Brown, TestSpecifications.Ø10_Brown }
            );

            var addStructureResult2 = await _commandDispatcher.HandleAsync<PlaceAdditionalStructuresInSpanEquipment, Result>(addStructure2);


            var equipmentQueryResult = await _queryDispatcher.HandleAsync<GetEquipmentDetails, Result<GetEquipmentDetailsResult>>(
               new GetEquipmentDetails(new EquipmentIdList() { sutSpanEquipmentId })
            );

            var equipmentAfterAddingStructure = equipmentQueryResult.Value.SpanEquipment[sutSpanEquipmentId];

            // Act
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(TestRouteNetwork.J_1));

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(getDiagramQueryResult.Value.Diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            getDiagramQueryResult.IsSuccess.Should().BeTrue();
            var diagram = getDiagramQueryResult.Value.Diagram;

            addStructureResult.IsSuccess.Should().BeTrue();
            addStructureResult2.IsSuccess.Should().BeTrue();
            getDiagramQueryResult.IsSuccess.Should().BeTrue();

            // Assert that no empty guids
            diagram.DiagramObjects.Any(d => d.IdentifiedObject != null && d.IdentifiedObject.RefId == Guid.Empty).Should().BeFalse();

            // Check that all added inner conduits are there
            diagram.DiagramObjects.Any(d => d.IdentifiedObject != null && d.IdentifiedObject.RefId == equipmentAfterAddingStructure.SpanStructures[1].SpanSegments[0].Id).Should().BeTrue();
            diagram.DiagramObjects.Any(d => d.IdentifiedObject != null && d.IdentifiedObject.RefId == equipmentAfterAddingStructure.SpanStructures[2].SpanSegments[0].Id).Should().BeTrue();
            diagram.DiagramObjects.Any(d => d.IdentifiedObject != null && d.IdentifiedObject.RefId == equipmentAfterAddingStructure.SpanStructures[3].SpanSegments[0].Id).Should().BeTrue();
            diagram.DiagramObjects.Any(d => d.IdentifiedObject != null && d.IdentifiedObject.RefId == equipmentAfterAddingStructure.SpanStructures[4].SpanSegments[0].Id).Should().BeTrue();

        }


        [Fact, Order(4)]
        public async void TestThatRemovedStructuresAreNotShownInDiagram()
        {
            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            var sutSpanEquipmentId = TestUtilityNetwork.FlexConduit_40_Red_SDU_1_to_SDU_2;

            utilityNetwork.TryGetEquipment<SpanEquipment>(sutSpanEquipmentId, out var sutSpanEquipment);

            // Remove inner conduit 1 from flexconduit
            var removeStructureCmd = new RemoveSpanStructureFromSpanEquipment(sutSpanEquipment.SpanStructures[1].SpanSegments[0].Id);

            var removeStructureCmdResult = await _commandDispatcher.HandleAsync<RemoveSpanStructureFromSpanEquipment, Result>(removeStructureCmd);


            // Act
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(TestRouteNetwork.J_1));

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(getDiagramQueryResult.Value.Diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            removeStructureCmdResult.IsSuccess.Should().BeTrue();
            getDiagramQueryResult.IsSuccess.Should().BeTrue();
            var diagram = getDiagramQueryResult.Value.Diagram;

            // Make sure the fist inner conduit is gone
            diagram.DiagramObjects.Any(d => d.IdentifiedObject != null && d.IdentifiedObject.RefId == sutSpanEquipment.SpanStructures[1].SpanSegments[0].Id).Should().BeFalse();

        }

        [Fact, Order(5)]
        public async void TestThatRemovedSpanEquipmentAreNotShownInDiagram()
        {
            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            var sutSpanEquipmentId = TestUtilityNetwork.MultiConduit_5x10_SDU_1_to_SDU_2;

            utilityNetwork.TryGetEquipment<SpanEquipment>(sutSpanEquipmentId, out var sutSpanEquipment);

            // Remove outer conduit (which means remove the whole thing)
            var removeStructureCmd = new RemoveSpanStructureFromSpanEquipment(sutSpanEquipment.SpanStructures[0].SpanSegments[0].Id);
            var removeStructureCmdResult = await _commandDispatcher.HandleAsync<RemoveSpanStructureFromSpanEquipment, Result>(removeStructureCmd);

            // Remember to remove the walk of interest as well
            var unregisterInterestCmd = new UnregisterInterest(sutSpanEquipment.WalkOfInterestId);
            var unregisterInterestCmdResult = await _commandDispatcher.HandleAsync<UnregisterInterest, Result>(unregisterInterestCmd);

            // Act
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(TestRouteNetwork.J_1));

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(getDiagramQueryResult.Value.Diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            removeStructureCmdResult.IsSuccess.Should().BeTrue();
            unregisterInterestCmdResult.IsSuccess.Should().BeTrue();
            getDiagramQueryResult.IsSuccess.Should().BeTrue();
            var diagram = getDiagramQueryResult.Value.Diagram;

            // Make sure the no inner conduit is gone
            diagram.DiagramObjects.Any(d => d.IdentifiedObject != null && d.IdentifiedObject.RefId == sutSpanEquipment.SpanStructures[0].SpanSegments[0].Id).Should().BeFalse();
            diagram.DiagramObjects.Any(d => d.IdentifiedObject != null && d.IdentifiedObject.RefId == sutSpanEquipment.SpanStructures[1].SpanSegments[0].Id).Should().BeFalse();
            diagram.DiagramObjects.Any(d => d.IdentifiedObject != null && d.IdentifiedObject.RefId == sutSpanEquipment.SpanStructures[2].SpanSegments[0].Id).Should().BeFalse();
            diagram.DiagramObjects.Any(d => d.IdentifiedObject != null && d.IdentifiedObject.RefId == sutSpanEquipment.SpanStructures[3].SpanSegments[0].Id).Should().BeFalse();
            diagram.DiagramObjects.Any(d => d.IdentifiedObject != null && d.IdentifiedObject.RefId == sutSpanEquipment.SpanStructures[4].SpanSegments[0].Id).Should().BeFalse();
            diagram.DiagramObjects.Any(d => d.IdentifiedObject != null && d.IdentifiedObject.RefId == sutSpanEquipment.SpanStructures[5].SpanSegments[0].Id).Should().BeFalse();

        }



    }
}
