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
    public class GetDiagramQueryTests
    {
        private readonly IQueryDispatcher _queryDispatcher;

        private static TestSpecifications _specs;
        private static TestConduits _conduits;

        public GetDiagramQueryTests(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;

            _specs = new TestSpecifications(commandDispatcher, queryDispatcher).Run();
            _conduits = new TestConduits(commandDispatcher, queryDispatcher).Run();
        }

        [Fact, Order(1)]
        public async void TestGetDiagramQueryOnCC_1()
        {
            var sutRouteNetworkElement = TestRouteNetwork.CC_1;

            // Act
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));

            // Assert
            getDiagramQueryResult.IsSuccess.Should().BeTrue();
            getDiagramQueryResult.Value.Diagram.DiagramObjects.Count().Should().BeGreaterThan(15);

            getDiagramQueryResult.Value.Diagram.Envelope.MinX.Should().Be(-0.01);
            getDiagramQueryResult.Value.Diagram.Envelope.MaxX.Should().Be(0.04);

        }

        [Fact, Order(2)]
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


        [Fact, Order(3)]
        public async void TestGetDiagramQueryOnNodeWithNoConduits_ShouldReturnEmptyDiagram()
        {
            var sutRouteNetworkElement = TestRouteNetwork.HH_11;

            // Act
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));

            // Assert
            getDiagramQueryResult.IsSuccess.Should().BeTrue();
            getDiagramQueryResult.Value.Diagram.DiagramObjects.Count().Should().Be(0);
        }

        [Fact, Order(3)]
        public async void TestGetDiagramForRouteNodeThatDontExist_ShouldFail()
        {
            var sutRouteNetworkElement = Guid.NewGuid();

            // Act
            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));

            // Assert
            getDiagramQueryResult.IsFailed.Should().BeTrue();
        }
    }
}
