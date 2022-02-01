using FluentAssertions;
using FluentResults;
using NetTopologySuite.Geometries;
using OpenFTTH.CQRS;
using OpenFTTH.Events.Core.Infos;
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
    [Order(5)]
    public class FiberCableTests
    {
        private readonly IEventStore _eventStore;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        private static TestUtilityNetwork _utilityNetwork;

        public FiberCableTests(IEventStore eventStore, ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _eventStore = eventStore;
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;

            new TestSpecifications(commandDispatcher, queryDispatcher).Run();
            _utilityNetwork = new TestUtilityNetwork(commandDispatcher, queryDispatcher).Run();
        }

        [Fact, Order(1)]
        public async void TestDrawingCableEndOutsideConduitInRouteNode()
        {
            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            var sutRouteNetworkElement = TestRouteNetwork.CC_1;

            // Setup
            var specs = new TestSpecifications(_commandDispatcher, _queryDispatcher).Run();

            // Cable directly in route network from HH_1 to CC_1
            var routingHops = new RoutingHop[]
            {
                new RoutingHop(
                    new Guid[] { TestRouteNetwork.HH_1, TestRouteNetwork.S2, TestRouteNetwork.HH_2, TestRouteNetwork.S4, TestRouteNetwork.CC_1 }
                ),
            };

            var placeSpanEquipmentCommand = new PlaceSpanEquipmentInUtilityNetwork(Guid.NewGuid(), new UserContext("test", Guid.Empty), Guid.NewGuid(), TestSpecifications.FiberCable_192Fiber, routingHops)
            {
                NamingInfo = new NamingInfo("K12345678", null),
                ManufacturerId = Guid.NewGuid()
            };

            // Act
            var placeSpanEquipmentResult = await _commandDispatcher.HandleAsync<PlaceSpanEquipmentInUtilityNetwork, Result>(placeSpanEquipmentCommand);

            utilityNetwork.TryGetEquipment<SpanEquipment>(placeSpanEquipmentCommand.SpanEquipmentId, out var placedSpanEquipment);


            // Act
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));

            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            diagram.DiagramObjects.Count(o => o.Style == "FiberCable" && o.Geometry is LineString).Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Label != null && o.Label.Contains("K12345678")).Should().Be(1);
        }



        [Fact, Order(2)]
        public async void TestDrawingCableOutsideConduitPassingThrougRouteSegment()
        {
            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            var sutRouteNetworkElement = TestRouteNetwork.S2;

            // Act
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));

            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            diagram.DiagramObjects.Count(o => o.Style == "FiberCable" && o.Geometry is LineString).Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Label != null && o.Label.Contains("K12345678")).Should().Be(1);
        }


        [Fact, Order(10)]
        public async void TestDrawingCableInsideConduitPassingThrougRouteSegment()
        {
            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            var sutRouteNetworkElement = TestRouteNetwork.S2;

            // The span equipment/segment where to route the child span equipment
            var routeThroughSpanEquipmentId = TestUtilityNetwork.MultiConduit_10x10_HH_1_to_HH_10;

            utilityNetwork.TryGetEquipment<SpanEquipment>(routeThroughSpanEquipmentId, out var routeThoughSpanEquipment);

            var routeThroughSpanSegmentId = routeThoughSpanEquipment.SpanStructures[10].SpanSegments[0].Id;

            // Setup command
            var specs = new TestSpecifications(_commandDispatcher, _queryDispatcher).Run();

            var routingHops = new RoutingHop[]
            {
                new RoutingHop(TestRouteNetwork.HH_1, routeThroughSpanSegmentId)
            };

            var placeSpanEquipmentCommand = new PlaceSpanEquipmentInUtilityNetwork(Guid.NewGuid(), new UserContext("test", Guid.Empty), Guid.NewGuid(), TestSpecifications.FiberCable_72Fiber, routingHops)
            {
                NamingInfo = new NamingInfo("K11112222", null),
                ManufacturerId = Guid.NewGuid()
            };

            // Act
            var placeSpanEquipmentResult = await _commandDispatcher.HandleAsync<PlaceSpanEquipmentInUtilityNetwork, Result>(placeSpanEquipmentCommand);
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));


            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            diagram.DiagramObjects.Count(o => o.Style == "FiberCable" && o.Geometry is LineString).Should().Be(2);
            diagram.DiagramObjects.Count(o => o.Label != null && o.Label.Contains("K11112222")).Should().Be(1);


        }


        [Fact, Order(11)]
        public async void TestDrawingCableInsideConduitStartingInNode()
        {
            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            var sutRouteNetworkElement = TestRouteNetwork.HH_1;

       
            // Act
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));


            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            diagram.DiagramObjects.Count(o => o.Style == "FiberCable" && o.Geometry is LineString).Should().Be(2);
            diagram.DiagramObjects.Count(o => o.Label != null && o.Label.Contains("K11112222")).Should().Be(1);


        }


        [Fact, Order(19)]
        public async void TestDrawingCableInConnectedConduitsInsideNodeContainer()
        {
            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            var sutRouteNetworkElement = TestRouteNetwork.CC_1;

            // The span equipment/segment where to route the child span equipment
            var routeThroughSpanEquipmentId = TestUtilityNetwork.MultiConduit_12x7_HH_1_to_HH_10;

            utilityNetwork.TryGetEquipment<SpanEquipment>(routeThroughSpanEquipmentId, out var routeThoughSpanEquipment);

            var routeThroughSpanSegmentId = routeThoughSpanEquipment.SpanStructures[2].SpanSegments[0].Id;

            // Setup command
            var specs = new TestSpecifications(_commandDispatcher, _queryDispatcher).Run();

            var routingHops = new RoutingHop[]
            {
                new RoutingHop(TestRouteNetwork.HH_1, routeThroughSpanSegmentId)
            };

            var placeSpanEquipmentCommand = new PlaceSpanEquipmentInUtilityNetwork(Guid.NewGuid(), new UserContext("test", Guid.Empty), Guid.NewGuid(), TestSpecifications.FiberCable_72Fiber, routingHops)
            {
                NamingInfo = new NamingInfo("K22223333", null),
                ManufacturerId = Guid.NewGuid()
            };

            // Act
            var placeSpanEquipmentResult = await _commandDispatcher.HandleAsync<PlaceSpanEquipmentInUtilityNetwork, Result>(placeSpanEquipmentCommand);
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));


            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            diagram.DiagramObjects.Count(o => o.Style == "FiberCable" && o.Geometry is LineString).Should().Be(3);


        }


        [Fact, Order(20)]
        public async void TestDrawingCableInPassThroughConduitInsideNodeContainer()
        {
            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            var sutRouteNetworkElement = TestRouteNetwork.CC_1;

            // The span equipment/segment where to route the child span equipment
            var routeThroughSpanEquipmentId = TestUtilityNetwork.MultiConduit_12x7_HH_1_to_HH_10;

            utilityNetwork.TryGetEquipment<SpanEquipment>(routeThroughSpanEquipmentId, out var routeThoughSpanEquipment);

            var routeThroughSpanSegmentId = routeThoughSpanEquipment.SpanStructures[10].SpanSegments[0].Id;

            // Setup command
            var specs = new TestSpecifications(_commandDispatcher, _queryDispatcher).Run();

            var routingHops = new RoutingHop[]
            {
                new RoutingHop(TestRouteNetwork.HH_1, routeThroughSpanSegmentId)
            };

            var placeSpanEquipmentCommand = new PlaceSpanEquipmentInUtilityNetwork(Guid.NewGuid(), new UserContext("test", Guid.Empty), Guid.NewGuid(), TestSpecifications.FiberCable_72Fiber, routingHops)
            {
                NamingInfo = new NamingInfo("K33334444", null),
                ManufacturerId = Guid.NewGuid()
            };

            // Act
            var placeSpanEquipmentResult = await _commandDispatcher.HandleAsync<PlaceSpanEquipmentInUtilityNetwork, Result>(placeSpanEquipmentCommand);
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));


            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            diagram.DiagramObjects.Count(o => o.Style == "FiberCable" && o.Geometry is LineString).Should().Be(4);


        }



        [Fact, Order(21)]
        public async void TestDrawingCableEndInsideNodeContainerCC1()
        {
            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            var sutRouteNetworkElement = TestRouteNetwork.CC_1;

            // The span equipment/segment where to route the child span equipment
            var routeThroughSpanEquipmentId = TestUtilityNetwork.MultiConduit_12x7_HH_1_to_HH_10;

            utilityNetwork.TryGetEquipment<SpanEquipment>(routeThroughSpanEquipmentId, out var routeThoughSpanEquipment);

            var routeThroughSpanSegmentId = routeThoughSpanEquipment.SpanStructures[4].SpanSegments[0].Id;

            // Setup command
            var specs = new TestSpecifications(_commandDispatcher, _queryDispatcher).Run();

            var routingHops = new RoutingHop[]
            {
                new RoutingHop(TestRouteNetwork.HH_1, routeThroughSpanSegmentId)
            };

            var placeSpanEquipmentCommand = new PlaceSpanEquipmentInUtilityNetwork(Guid.NewGuid(), new UserContext("test", Guid.Empty), Guid.NewGuid(), TestSpecifications.FiberCable_288Fiber, routingHops)
            {
                NamingInfo = new NamingInfo("K66660000", null),
                ManufacturerId = Guid.NewGuid()
            };

            // Act
            var placeSpanEquipmentResult = await _commandDispatcher.HandleAsync<PlaceSpanEquipmentInUtilityNetwork, Result>(placeSpanEquipmentCommand);
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));


            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            diagram.DiagramObjects.Count(o => o.Style == "FiberCable" && o.Geometry is LineString).Should().Be(5);


        }


        [Fact, Order(22)]
        public async void TestDrawingCableThrowhWellInsideNodeContainerCC1()
        {
            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            var sutRouteNetworkElement = TestRouteNetwork.CC_1;

            // The span equipment/segment where to route the child span equipment
            var routeThroughSpanEquipmentId = TestUtilityNetwork.MultiConduit_12x7_HH_1_to_HH_10;
            var routeThroughSpanEquipmentId2 = TestUtilityNetwork.MultiConduit_3x10_CC_1_to_SP_1;

            utilityNetwork.TryGetEquipment<SpanEquipment>(routeThroughSpanEquipmentId, out var routeThoughSpanEquipment);
            utilityNetwork.TryGetEquipment<SpanEquipment>(routeThroughSpanEquipmentId2, out var routeThoughSpanEquipment2);

            var routeThroughSpanSegmentId = routeThoughSpanEquipment.SpanStructures[3].SpanSegments[0].Id;
            var routeThroughSpanSegmentId2 = routeThoughSpanEquipment2.SpanStructures[2].SpanSegments[0].Id;

            // Setup command
            var specs = new TestSpecifications(_commandDispatcher, _queryDispatcher).Run();

            var routingHops = new RoutingHop[]
            {
                new RoutingHop(TestRouteNetwork.HH_1, routeThroughSpanSegmentId),
                new RoutingHop(TestRouteNetwork.CC_1, routeThroughSpanSegmentId2)
            };

            var placeSpanEquipmentCommand = new PlaceSpanEquipmentInUtilityNetwork(Guid.NewGuid(), new UserContext("test", Guid.Empty), Guid.NewGuid(), TestSpecifications.FiberCable_288Fiber, routingHops)
            {
                NamingInfo = new NamingInfo("K99991111", null),
                ManufacturerId = Guid.NewGuid()
            };

            // Act
            var placeSpanEquipmentResult = await _commandDispatcher.HandleAsync<PlaceSpanEquipmentInUtilityNetwork, Result>(placeSpanEquipmentCommand);
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));


            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            diagram.DiagramObjects.Count(o => o.Style == "FiberCable" && o.Geometry is LineString).Should().Be(6);
        }


        [Fact, Order(23)]
        public async void TestDrawingCableThrowSingleConduitInWellInsideNodeContainerCC1()
        {
            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            var sutRouteNetworkElement = TestRouteNetwork.CC_1;

            // The span equipment/segment where to route the child span equipment
            var routeThroughSpanEquipmentId = TestUtilityNetwork.MultiConduit_12x7_HH_1_to_HH_10;
            var routeThroughSpanEquipmentId2 = TestUtilityNetwork.CustomerConduit_CC_1_to_SDU_1;

            utilityNetwork.TryGetEquipment<SpanEquipment>(routeThroughSpanEquipmentId, out var routeThoughSpanEquipment);
            utilityNetwork.TryGetEquipment<SpanEquipment>(routeThroughSpanEquipmentId2, out var routeThoughSpanEquipment2);

            var routeThroughSpanSegmentId = routeThoughSpanEquipment.SpanStructures[6].SpanSegments[0].Id;
            var routeThroughSpanSegmentId2 = routeThoughSpanEquipment2.SpanStructures[0].SpanSegments[0].Id;

            // Setup command
            var specs = new TestSpecifications(_commandDispatcher, _queryDispatcher).Run();

            var routingHops = new RoutingHop[]
            {
                new RoutingHop(TestRouteNetwork.HH_1, routeThroughSpanSegmentId)
            };

            var placeSpanEquipmentCommand = new PlaceSpanEquipmentInUtilityNetwork(Guid.NewGuid(), new UserContext("test", Guid.Empty), Guid.NewGuid(), TestSpecifications.FiberCable_288Fiber, routingHops)
            {
                NamingInfo = new NamingInfo("K999922222", null),
                ManufacturerId = Guid.NewGuid()
            };

            // Act
            var placeSpanEquipmentResult = await _commandDispatcher.HandleAsync<PlaceSpanEquipmentInUtilityNetwork, Result>(placeSpanEquipmentCommand);
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));

            // Assert
            placeSpanEquipmentResult.IsSuccess.Should().BeTrue();

            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            diagram.DiagramObjects.Count(o => o.Style == "FiberCable" && o.Geometry is LineString).Should().Be(7);

            diagram.DiagramObjects.Count(o => o.Style == "OuterConduitOrange" && o.Geometry is Polygon).Should().Be(9);


        }


        [Fact, Order(24)]
        public async void TestDrawingCableThroughWellInsideNodeContainerHH2()
        {
            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            var sutRouteNetworkElement = TestRouteNetwork.HH_2;


            // The span equipment/segment where to route the child span equipment
            var routeThroughSpanEquipmentId = TestUtilityNetwork.MultiConduit_12x10_5x10_HH_1_to_HH_2;
            var routeThroughSpanEquipmentId2 = TestUtilityNetwork.FlexConduit_40_Red_HH_2_to_FP_2;

            utilityNetwork.TryGetEquipment<SpanEquipment>(routeThroughSpanEquipmentId, out var routeThoughSpanEquipment);
            utilityNetwork.TryGetEquipment<SpanEquipment>(routeThroughSpanEquipmentId2, out var routeThoughSpanEquipment2);

            var flexConduitSpanSegmentId = routeThoughSpanEquipment2.SpanStructures[0].SpanSegments[0].Id;


            // Cable 1
            var routingHops = new RoutingHop[]
            {
                new RoutingHop(TestRouteNetwork.HH_1, routeThoughSpanEquipment.SpanStructures[3].SpanSegments[0].Id),
                new RoutingHop(TestRouteNetwork.HH_2, flexConduitSpanSegmentId)
            };

            var placeSpanEquipmentCommand = new PlaceSpanEquipmentInUtilityNetwork(Guid.NewGuid(), new UserContext("test", Guid.Empty), Guid.NewGuid(), TestSpecifications.FiberCable_288Fiber, routingHops)
            {
                NamingInfo = new NamingInfo("K66600000", null),
                ManufacturerId = Guid.NewGuid()
            };

            var placeSpanEquipmentResult1 = await _commandDispatcher.HandleAsync<PlaceSpanEquipmentInUtilityNetwork, Result>(placeSpanEquipmentCommand);


            // Cable 2
            var routingHops2 = new RoutingHop[]
            {
                new RoutingHop(TestRouteNetwork.HH_1, routeThoughSpanEquipment.SpanStructures[4].SpanSegments[0].Id),
                new RoutingHop(TestRouteNetwork.HH_2, flexConduitSpanSegmentId)
            };

            var placeSpanEquipmentCommand2 = new PlaceSpanEquipmentInUtilityNetwork(Guid.NewGuid(), new UserContext("test", Guid.Empty), Guid.NewGuid(), TestSpecifications.FiberCable_288Fiber, routingHops2)
            {
                NamingInfo = new NamingInfo("K66700000", null),
                ManufacturerId = Guid.NewGuid()
            };

            var placeSpanEquipmentResult2 = await _commandDispatcher.HandleAsync<PlaceSpanEquipmentInUtilityNetwork, Result>(placeSpanEquipmentCommand2);


            // Cable 3
            var routingHops3 = new RoutingHop[]
            {
                new RoutingHop(TestRouteNetwork.HH_2, flexConduitSpanSegmentId),
                new RoutingHop(TestRouteNetwork.HH_1, routeThoughSpanEquipment.SpanStructures[1].SpanSegments[0].Id),
            };

            var placeSpanEquipmentCommand3 = new PlaceSpanEquipmentInUtilityNetwork(Guid.NewGuid(), new UserContext("test", Guid.Empty), Guid.NewGuid(), TestSpecifications.FiberCable_288Fiber, routingHops3)
            {
                NamingInfo = new NamingInfo("K66700001", null),
                ManufacturerId = Guid.NewGuid()
            };

            var placeSpanEquipmentResult3 = await _commandDispatcher.HandleAsync<PlaceSpanEquipmentInUtilityNetwork, Result>(placeSpanEquipmentCommand3);



            // Assert
            placeSpanEquipmentResult1.IsSuccess.Should().BeTrue();
            placeSpanEquipmentResult2.IsSuccess.Should().BeTrue();

            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));


            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            diagram.DiagramObjects.Count(o => o.Style == "FiberCable" && o.Geometry is LineString).Should().Be(10);
        }



        [Fact, Order(31)]
        public async void TestDrawingCableThroughOuterConduitInsideNodeSegment()
        {
            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            var sutRouteNetworkElement = TestRouteNetwork.S3;

         
            // Assert
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));


            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            diagram.DiagramObjects.Count(o => o.Style == "FiberCable" && o.Geometry is LineString).Should().Be(3);
        }



        [Fact, Order(30)]
        public async void TestDrawingCableThroughOuterConduitEndInsideNode()
        {
            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            var sutRouteNetworkElement = TestRouteNetwork.FP_2;


            // Assert
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));


            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            diagram.DiagramObjects.Count(o => o.Style == "FiberCable" && o.Geometry is LineString).Should().Be(3);
        }
    }
}
