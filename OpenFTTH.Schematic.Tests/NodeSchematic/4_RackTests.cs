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
    [Order(4)]
    public class RackTests
    {
        private readonly IEventStore _eventStore;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        private static TestUtilityNetwork _utilityNetwork;

        public RackTests(IEventStore eventStore, ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _eventStore = eventStore;
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;

            new TestSpecifications(commandDispatcher, queryDispatcher).Run();
            _utilityNetwork = new TestUtilityNetwork(commandDispatcher, queryDispatcher).Run();
        }

        [Fact, Order(1)]
        public async void TestDrawingFP2_WithNoEquipments()
        {
            var sutRouteNetworkElement = TestRouteNetwork.FP_2;

            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            // Act
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));

            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert

            getDiagramQueryResult.IsSuccess.Should().BeTrue();

            diagram.DiagramObjects.Count(o => o.Style == "Rack").Should().Be(0);
            diagram.DiagramObjects.Count(o => o.Style == "SubrackSpace").Should().Be(0);
            diagram.DiagramObjects.Count(o => o.Style == "RackLabel").Should().Be(0);
            diagram.DiagramObjects.Count(o => o.Style == "RackUnitLabel").Should().Be(0);
        }


        [Fact, Order(10)]
        public async void TestDrawingOneRackInFP2()
        {
            var sutRouteNetworkElement = TestRouteNetwork.FP_2;

            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            // Act
            var placeRackCmd = new PlaceRackInNodeContainer(
              Guid.NewGuid(),
              new UserContext("test", Guid.Empty),
              TestUtilityNetwork.NodeContainer_FP_2,
              TestSpecifications.Rack_ESTI,
              "Rack 1",
              80
            );

            var placeRackResult = await _commandDispatcher.HandleAsync<PlaceRackInNodeContainer, Result>(placeRackCmd);

            // Assert
            placeRackResult.IsSuccess.Should().BeTrue();



            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));

            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert

            getDiagramQueryResult.IsSuccess.Should().BeTrue();

            diagram.DiagramObjects.Count(o => o.Style == "Rack").Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "SubrackSpace").Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "RackLabel").Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "RackUnitLabel").Should().Be(81);
        }

        [Fact, Order(11)]
        public async void TestAddOneMoreRackInFP2()
        {
            var sutRouteNetworkElement = TestRouteNetwork.FP_2;

            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            // Act
            var placeRackCmd = new PlaceRackInNodeContainer(
              Guid.NewGuid(),
              new UserContext("test", Guid.Empty),
              TestUtilityNetwork.NodeContainer_FP_2,
              TestSpecifications.Rack_ESTI,
              "Rack 2",
              80
            );

            var placeRackResult = await _commandDispatcher.HandleAsync<PlaceRackInNodeContainer, Result>(placeRackCmd);

            // Assert
            placeRackResult.IsSuccess.Should().BeTrue();



            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));

            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            getDiagramQueryResult.IsSuccess.Should().BeTrue();

            diagram.DiagramObjects.Count(o => o.Style == "Rack").Should().Be(2);
            diagram.DiagramObjects.Count(o => o.Style == "SubrackSpace").Should().Be(2);
            diagram.DiagramObjects.Count(o => o.Style == "RackLabel").Should().Be(2);
            diagram.DiagramObjects.Count(o => o.Style == "RackUnitLabel").Should().Be(162);
        }




    }
}
