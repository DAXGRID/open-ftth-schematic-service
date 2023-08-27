using FluentAssertions;
using FluentResults;
using OpenFTTH.CQRS;
using OpenFTTH.Events.Core.Infos;
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
    [Order(7)]
    public class CableInOuterConduitTests
    {
        private readonly IEventStore _eventStore;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        private static TestUtilityNetwork _utilityNetwork;

        private Guid _cable1 = Guid.Parse("1d487bbf-c909-45b2-83b9-1181e995da00");
        private Guid _cable2 = Guid.Parse("6d86fdfd-e786-4fd5-823a-a5e90f8970ee");

        public CableInOuterConduitTests(IEventStore eventStore, ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _eventStore = eventStore;
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;

            new TestSpecifications(commandDispatcher, queryDispatcher).Run();
            _utilityNetwork = new TestUtilityNetwork(commandDispatcher, queryDispatcher).Run();
        }

        [Fact, Order(1)]
        public async void TestCableFromOuterConduitToOuterConduitInSP1()
        {
            var sutRouteNetworkElement = TestRouteNetwork.SP_1;
        
            var utilityNetwork = _eventStore.Projections.Get<UtilityNetworkProjection>();

            PlaceCableBetweenCC1AndJ1(_commandDispatcher, _cable1, "flex-flex-1");
            PlaceCableBetweenCC1AndJ1(_commandDispatcher, _cable2, "flex-flex-2");

            TestUtilityNetwork.AffixCableToSingleConduit(_eventStore, _commandDispatcher, TestRouteNetwork.SP_1, _cable1, TestUtilityNetwork.FlexConduit_40_Red_CC_1_to_SP_1);
            TestUtilityNetwork.AffixCableToSingleConduit(_eventStore, _commandDispatcher, TestRouteNetwork.SP_1, _cable1, TestUtilityNetwork.FlexConduit_40_Red_SP_1_to_J_1);

            TestUtilityNetwork.AffixCableToSingleConduit(_eventStore, _commandDispatcher, TestRouteNetwork.SP_1, _cable2, TestUtilityNetwork.FlexConduit_40_Red_CC_1_to_SP_1);
            TestUtilityNetwork.AffixCableToSingleConduit(_eventStore, _commandDispatcher, TestRouteNetwork.SP_1, _cable2, TestUtilityNetwork.FlexConduit_40_Red_SP_1_to_J_1);




            var getDiagramQueryResult = await _queryDispatcher.HandleAsync<GetDiagram, Result<GetDiagramResult>>(new GetDiagram(sutRouteNetworkElement));
            getDiagramQueryResult.IsSuccess.Should().BeTrue();

            var diagram = getDiagramQueryResult.Value.Diagram;

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert that engum møllevej is shown 2 times in CC
            diagram.DiagramObjects.Count(o => o.Label == "Engum Møllevej 3").Should().Be(2);


        }


        private Guid PlaceCableBetweenCC1AndJ1(ICommandDispatcher commandDispatcher, Guid cableId, string name)
        {
            // Cable directly in route network from HH_1 to CC_1
            var routingHops = new RoutingHop[]
            {
                new RoutingHop(
                    new Guid[] { TestRouteNetwork.S5, TestRouteNetwork.S6 }
                ),
            };

         
            var placeSpanEquipmentCommand = new PlaceSpanEquipmentInUtilityNetwork(Guid.NewGuid(), new UserContext("test", Guid.Empty),cableId, TestSpecifications.FiberCable_2Fiber, routingHops)
            {
                NamingInfo = new NamingInfo(name, null),
                ManufacturerId = Guid.NewGuid()
            };

            // Act
            var placeSpanEquipmentResult = commandDispatcher.HandleAsync<PlaceSpanEquipmentInUtilityNetwork, Result>(placeSpanEquipmentCommand).Result;

            return cableId;
        }








    }
}
