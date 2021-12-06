﻿using FluentAssertions;
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


        [Fact, Order(20)]
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



    }
}
