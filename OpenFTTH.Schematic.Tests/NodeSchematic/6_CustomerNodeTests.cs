using FluentAssertions;
using FluentResults;
using OpenFTTH.CQRS;
using OpenFTTH.EventSourcing;
using OpenFTTH.Schematic.API.Queries;
using OpenFTTH.Schematic.Business.IO;
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
    [Order(6)]
    public class CustomerNodeTests
    {
        private readonly IEventStore _eventStore;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        private static TestUtilityNetwork _utilityNetwork;

        public CustomerNodeTests(IEventStore eventStore, ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _eventStore = eventStore;
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;

            new TestSpecifications(commandDispatcher, queryDispatcher).Run();
            _utilityNetwork = new TestUtilityNetwork(commandDispatcher, queryDispatcher).Run();
        }

        [Fact, Order(1)]
        public async void TestDrawingAddressesInCC()
        {
            var sutRouteNetworkElement = TestRouteNetwork.CC_1;
            var sutSpanEquipment = TestUtilityNetwork.CustomerConduit_CC_1_to_SDU_1;

            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));
            getDiagramQueryResult.IsSuccess.Should().BeTrue();

            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert that engum møllevej is shown 2 times in CC
            diagram.DiagramObjects.Count(o => o.Label == "Engum Møllevej 3").Should().Be(2);


        }

        [Fact, Order(2)]
        public async void TestDrawingAddressesInCustomerTermination()
        {
            var sutRouteNetworkElement = TestRouteNetwork.SDU_1;
            var sutSpanEquipment = TestUtilityNetwork.CustomerConduit_CC_1_to_SDU_1;

            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));
            getDiagramQueryResult.IsSuccess.Should().BeTrue();

            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert that engum møllevej is shown 1 times in SDU 1
            diagram.DiagramObjects.Count(o => o.Label == "Engum Møllevej 3").Should().Be(2);


        }

        [Fact, Order(3)]
        public async void TestAddCustomerTerminationInSDU_1()
        {
            var sutRouteNetworkElement = TestRouteNetwork.SDU_1;
            var sutNodeContainer = TestUtilityNetwork.NodeContainer_SDU_1;

            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            utilityNetwork.TryGetEquipment<NodeContainer>(sutNodeContainer, out var nodeContainerBeforeCommand);

            // Act
            var placeEquipmentCmd = new PlaceTerminalEquipmentInNodeContainer(
                terminalEquipmentId: Guid.NewGuid(),
                correlationId: Guid.NewGuid(),
                userContext: new UserContext("test", Guid.Empty),
                nodeContainerId: sutNodeContainer,
                terminalEquipmentSpecificationId: TestSpecifications.CustomerTermination,
                numberOfEquipments: 1,
                startSequenceNumber: 1,
                namingMethod: TerminalEquipmentNamingMethodEnum.NameOnly,
                namingInfo: new Events.Core.Infos.NamingInfo() { Name = "I12345" }
            );

            var placeEquipmentResult = await _commandDispatcher.HandleAsync<PlaceTerminalEquipmentInNodeContainer, Result>(placeEquipmentCmd);

            // Assert
            placeEquipmentResult.IsSuccess.Should().BeTrue();


            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));

            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            getDiagramQueryResult.IsSuccess.Should().BeTrue();

            diagram.DiagramObjects.Count(o => o.Style == "TerminalEquipmentWithProperties").Should().Be(1);

        }






    }
}
