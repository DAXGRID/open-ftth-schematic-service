using FluentAssertions;
using FluentResults;
using OpenFTTH.CQRS;
using OpenFTTH.RouteNetwork.API.Model;
using OpenFTTH.RouteNetwork.API.Queries;
using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.API.Queries;
using OpenFTTH.Schematic.Business.IO;
using OpenFTTH.Schematic.Business.SchematicBuilder;
using OpenFTTH.TestData;
using OpenFTTH.Util;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using OpenFTTH.UtilityGraphService.API.Queries;
using System;
using System.Linq;
using Xunit;
using Xunit.Extensions.Ordering;

namespace OpenFTTH.Schematic.Tests.NodeSchematic
{

    [Order(1)]
    public class GetDiagramQueryTests
    {
        private readonly IQueryDispatcher _queryDispatcher;

        private static TestSpecifications _specs;
        private static TestUtilityNetwork _conduits;

        public GetDiagramQueryTests(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;

            new TestSpecifications(commandDispatcher, queryDispatcher).Run();
            _conduits = new TestUtilityNetwork(commandDispatcher, queryDispatcher).Run();
        }

        [Fact, Order(1)]
        public async void TestGetDiagramQueryOnCC_1()
        {
            var sutRouteNetworkElement = TestRouteNetwork.CC_1;

            // Act
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(getDiagramQueryResult.Value.Diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            getDiagramQueryResult.IsSuccess.Should().BeTrue();
            var diagram = getDiagramQueryResult.Value.Diagram;

            diagram.DiagramObjects.Count().Should().BeGreaterThan(15);
            diagram.Envelope.MinX.Should().Be(-0.01);
            diagram.Envelope.MaxX.Should().Be(0.04);

            // Check that node container has 4 sides with identified objects pointing to node container object
            diagram.DiagramObjects.Count(o => o.Style == "NodeContainer").Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "NodeContainerSideWest").Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "NodeContainerSideNorth").Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "NodeContainerSideEast").Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style == "NodeContainerSideSouth").Should().Be(1);
            diagram.DiagramObjects.Count(o => o.Style.StartsWith("NodeContainerSide") && o.IdentifiedObject.RefClass == "NodeContainer" && o.IdentifiedObject.RefId != Guid.Empty).Should().Be(4);


            // Check that 3x10 is rendered as an conduit heading towards in SP-1
            diagram.DiagramObjects.Count(o => o.Style == "SouthTerminalLabel" && o.Label == "SP-1").Should().Be(3);

            // Check that 10x10 and 5x10 is rendered as an conduit comming fron HH-1
            diagram.DiagramObjects.Count(o => o.Style == "WestTerminalLabel" && o.Label == "HH-1").Should().Be(22);
        }

        [Fact, Order(20)]
        public async void TestGetDiagramQueryOnHH_2()
        {
            var sutRouteNetworkElement = TestRouteNetwork.HH_2;

            // Act
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));

            // Assert
            getDiagramQueryResult.IsSuccess.Should().BeTrue();
            getDiagramQueryResult.Value.Diagram.DiagramObjects.Count().Should().BeGreaterThan(15);

            getDiagramQueryResult.Value.Diagram.Envelope.MinX.Should().Be(-0.01);
            getDiagramQueryResult.Value.Diagram.Envelope.MaxX.Should().Be(0.04);

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(getDiagramQueryResult.Value.Diagram).Export("c:/temp/diagram/test.geojson");
        }

        [Fact, Order(21)]
        public async void TestGetDiagramQueryOnHH_1()
        {
            var sutRouteNetworkElement = TestRouteNetwork.HH_1;

            // Act
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(getDiagramQueryResult.Value.Diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            getDiagramQueryResult.IsSuccess.Should().BeTrue();
            var diagram = getDiagramQueryResult.Value.Diagram;

            // Check that no conduit are drawed and labelled to end in the CC-1 node (because this means they are shown as pass-throughs, instead of conduit just ending in the node, which is wrong)
            diagram.DiagramObjects.Count(o => o.Label == "HH-10").Should().Be(22);
            diagram.DiagramObjects.Count(o => o.Label == "HH-1").Should().Be(0);
        }

        [Fact, Order(22)]
        public async void TestGetDiagramQueryOnHH_10()
        {
            var sutRouteNetworkElement = TestRouteNetwork.HH_10;

            // Act
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(getDiagramQueryResult.Value.Diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            getDiagramQueryResult.IsSuccess.Should().BeTrue();
            var diagram = getDiagramQueryResult.Value.Diagram;

            // Check that no conduit are drawed and labelled to end in the CC-1 node (because this means they are shown as pass-throughs, instead of conduit just ending in the node, which is wrong)
            diagram.DiagramObjects.Count(o => o.Label == "HH-1").Should().Be(22);
            diagram.DiagramObjects.Count(o => o.Label == "HH-10").Should().Be(1);
        }

        [Fact, Order(31)]
        public async void TestGetDiagramForRouteNodeThatDontExist_ShouldFail()
        {
            var sutRouteNetworkElement = Guid.NewGuid();

            // Act
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));

            // Assert
            getDiagramQueryResult.IsFailed.Should().BeTrue();
        }

        [Fact, Order(32)]
        public async void TestGetDiagramQueryOnNodeWithNodeContainerOnly_ShouldReturnNodeContainerInDiagram()
        {
            var sutRouteNetworkElement = TestRouteNetwork.J_1;

            // Act
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));

            // Assert
            getDiagramQueryResult.IsSuccess.Should().BeTrue();
            getDiagramQueryResult.Value.Diagram.DiagramObjects.Any(d => d.Style == "NodeContainer").Should().BeTrue();
        }


       
    }
}
