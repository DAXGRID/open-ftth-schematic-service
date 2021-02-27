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
using System.Linq;
using Xunit;
using Xunit.Extensions.Ordering;

namespace OpenFTTH.Schematic.Tests.NodeSchematic
{
    public class GetDiagramQueryTests
    {
        private readonly IQueryDispatcher _queryDispatcher;

        private static TestConduitSpecifications _conduitSpecs;
        private static TestConduits _conduits;

        public GetDiagramQueryTests(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;

            _conduitSpecs = new TestConduitSpecifications(commandDispatcher, queryDispatcher).Run();
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
    }
}
