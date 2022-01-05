using FluentResults;
using OpenFTTH.CQRS;
using OpenFTTH.Events.Core.Infos;
using OpenFTTH.RouteNetwork.API.Commands;
using OpenFTTH.RouteNetwork.API.Model;
using OpenFTTH.TestData;
using OpenFTTH.UtilityGraphService.API.Commands;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using System;
using System.Linq;
using System.Threading;

namespace OpenFTTH.TestData
{
    public class TestUtilityNetwork
    {
        private static bool _conduitsCreated = false;
        private static object _myLock = new object();

        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public static Guid MultiConduit_12x7_HH_1_to_HH_10;
        public static Guid MultiConduit_10x10_HH_1_to_HH_10;
        public static Guid FlexConduit_40_Red_HH_2_to_FP_2;
        public static Guid FlexConduit_40_Red_CC_1_to_SP_1;
        public static Guid MultiConduit_3x10_CC_1_to_SP_1;
        public static Guid MultiConduit_3x10_CC_1_to_HH_11;
        public static Guid FlexConduit_40_Red_SDU_1_to_SDU_2;
        public static Guid MultiConduit_5x10_SDU_1_to_SDU_2;
        public static Guid MultiConduit_5x10_SDU_1_to_J_1;
        public static Guid MultiConduit_5x10_SDU_2_to_J_1;
        public static Guid MultiConduit_12x10_5x10_HH_1_to_HH_2;

        public static Guid CustomerConduit_CC_1_to_SDU_1;
        public static Guid CustomerConduit_CC_1_to_SDU_2;
        public static Guid CustomerConduit_CC_1_to_SDU_3;
        public static Guid SingleConduit_CC_1_to_HH_10;

        public static Guid NodeContainer_HH_1;
        public static Guid NodeContainer_HH_2;
        public static Guid NodeContainer_HH_10;
        public static Guid NodeContainer_CC_1;
        public static Guid NodeContainer_J_1;
        public static Guid NodeContainer_FP_2;

        public TestUtilityNetwork(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _ = new TestSpecifications(commandDispatcher, queryDispatcher);
        }

        public TestUtilityNetwork Run()
        {
            if (_conduitsCreated)
                return this;

            lock (_myLock)
            {
                // double-checked locking
                if (_conduitsCreated)
                    return this;

                // Place some conduits in the route network we can play with
                MultiConduit_12x7_HH_1_to_HH_10 = PlaceConduit(TestSpecifications.Multi_Ø40_12x7, new RouteNetworkElementIdList() { TestRouteNetwork.S2, TestRouteNetwork.S4, TestRouteNetwork.S13 });
                MultiConduit_10x10_HH_1_to_HH_10 = PlaceConduit(TestSpecifications.Multi_Ø50_10x10, new RouteNetworkElementIdList() { TestRouteNetwork.S2, TestRouteNetwork.S4, TestRouteNetwork.S13 });
                FlexConduit_40_Red_HH_2_to_FP_2 = PlaceConduit(TestSpecifications.Flex_Ø40_Red, new RouteNetworkElementIdList() { TestRouteNetwork.S3 });
                FlexConduit_40_Red_CC_1_to_SP_1 = PlaceConduit(TestSpecifications.Flex_Ø40_Red, new RouteNetworkElementIdList() { TestRouteNetwork.S5 });
                MultiConduit_3x10_CC_1_to_SP_1 = PlaceConduit(TestSpecifications.Multi_Ø32_3x10, new RouteNetworkElementIdList() { TestRouteNetwork.S5 });
                MultiConduit_3x10_CC_1_to_HH_11 = PlaceConduit(TestSpecifications.Multi_Ø32_3x10, new RouteNetworkElementIdList() { TestRouteNetwork.S5, TestRouteNetwork.S6, TestRouteNetwork.S9, TestRouteNetwork.S11 });
                FlexConduit_40_Red_SDU_1_to_SDU_2 = PlaceConduit(TestSpecifications.Flex_Ø40_Red, new RouteNetworkElementIdList() { TestRouteNetwork.S7, TestRouteNetwork.S8 });
                MultiConduit_5x10_SDU_1_to_SDU_2 = PlaceConduit(TestSpecifications.Multi_Ø40_5x10, new RouteNetworkElementIdList() { TestRouteNetwork.S7, TestRouteNetwork.S8 });
                MultiConduit_5x10_SDU_1_to_J_1 = PlaceConduit(TestSpecifications.Multi_Ø40_5x10, new RouteNetworkElementIdList() { TestRouteNetwork.S7 });
                MultiConduit_5x10_SDU_2_to_J_1 = PlaceConduit(TestSpecifications.Multi_Ø40_5x10, new RouteNetworkElementIdList() { TestRouteNetwork.S8 });

                MultiConduit_12x10_5x10_HH_1_to_HH_2 = PlaceConduit(TestSpecifications.Multi_Ø50_12x7_5x10, new RouteNetworkElementIdList() { TestRouteNetwork.S2 });

                // Place customer conduit 1 (engum møllevej external unit address id)
                CustomerConduit_CC_1_to_SDU_1 = PlaceConduit(
                    TestSpecifications.CustomerConduit_Ø12_Orange, 
                    new RouteNetworkElementIdList() { TestRouteNetwork.S5, TestRouteNetwork.S6, TestRouteNetwork.S7 }, 
                    new AddressInfo() { UnitAddressId = Guid.Parse("0a3f50bc-aa89-32b8-e044-0003ba298018") }
                );

                // Place customer conduit 2 (Vesterbrogade 7A, Hedensted external access address id)
                CustomerConduit_CC_1_to_SDU_2 = PlaceConduit(
                    TestSpecifications.CustomerConduit_Ø12_Orange, 
                    new RouteNetworkElementIdList() { TestRouteNetwork.S5, TestRouteNetwork.S6, TestRouteNetwork.S8 },
                    new AddressInfo() { AccessAddressId = Guid.Parse("0a3f508f-8504-32b8-e044-0003ba298018") }
                );

                // Place customer conduit 3 (address remark)
                CustomerConduit_CC_1_to_SDU_3 = PlaceConduit(
                    TestSpecifications.CustomerConduit_Ø12_Orange,
                    new RouteNetworkElementIdList() { TestRouteNetwork.S5, TestRouteNetwork.S6, TestRouteNetwork.S9, TestRouteNetwork.S10 },
                    new AddressInfo() { Remark = "This conduit and at SDU 3" }
                );

                // Place single conduit
                SingleConduit_CC_1_to_HH_10 = PlaceConduit(TestSpecifications.CustomerConduit_Ø12_Orange, new RouteNetworkElementIdList() { TestRouteNetwork.S13, TestRouteNetwork.S5, TestRouteNetwork.S6, TestRouteNetwork.S8 });
                

                // Place node containers
                NodeContainer_HH_1 = PlaceNodeContainer(TestSpecifications.Well_Fiberpowertech_37_EK_378_400x800, TestSpecifications.Manu_Fiberpowertech, TestRouteNetwork.HH_1);
                NodeContainer_HH_2 = PlaceNodeContainer(TestSpecifications.Well_Fiberpowertech_37_EK_378_400x800, TestSpecifications.Manu_Fiberpowertech, TestRouteNetwork.HH_2);
                NodeContainer_HH_10 = PlaceNodeContainer(TestSpecifications.Well_Fiberpowertech_37_EK_378_400x800, TestSpecifications.Manu_Fiberpowertech, TestRouteNetwork.HH_10);
                NodeContainer_CC_1 = PlaceNodeContainer(TestSpecifications.Conduit_Closure_Emtelle_Branch_Box, TestSpecifications.Manu_Emtelle, TestRouteNetwork.CC_1);
                NodeContainer_J_1 = PlaceNodeContainer(TestSpecifications.Conduit_Closure_Emtelle_Branch_Box, TestSpecifications.Manu_Emtelle, TestRouteNetwork.J_1);
                NodeContainer_FP_2 = PlaceNodeContainer(TestSpecifications.Well_Cubis_STAKKAbox_MODULA_600x450, TestSpecifications.Manu_Emtelle, TestRouteNetwork.FP_2);


                // Affix conduits in H_2
                AffixSpanEquipmentToContainer(FlexConduit_40_Red_HH_2_to_FP_2, NodeContainer_HH_2, NodeContainerSideEnum.North);
                AffixSpanEquipmentToContainer(MultiConduit_12x10_5x10_HH_1_to_HH_2, NodeContainer_HH_2, NodeContainerSideEnum.West);

                // Affix 5x10 and 3x10 in CC 1
                AffixSpanEquipmentToContainer(MultiConduit_12x7_HH_1_to_HH_10, NodeContainer_CC_1, NodeContainerSideEnum.West);
                AffixSpanEquipmentToContainer(MultiConduit_3x10_CC_1_to_SP_1, NodeContainer_CC_1, NodeContainerSideEnum.South);
                AffixSpanEquipmentToContainer(MultiConduit_3x10_CC_1_to_HH_11, NodeContainer_CC_1, NodeContainerSideEnum.West);

                // Affix conduits in J_1
                AffixSpanEquipmentToContainer(MultiConduit_3x10_CC_1_to_HH_11, NodeContainer_J_1, NodeContainerSideEnum.West);
                AffixSpanEquipmentToContainer(MultiConduit_5x10_SDU_2_to_J_1, NodeContainer_J_1, NodeContainerSideEnum.West);
                AffixSpanEquipmentToContainer(FlexConduit_40_Red_SDU_1_to_SDU_2, NodeContainer_J_1, NodeContainerSideEnum.West);

                // Affix customer conduit in CC_1
                AffixSpanEquipmentToContainer(CustomerConduit_CC_1_to_SDU_1, NodeContainer_CC_1, NodeContainerSideEnum.North);
                // Affix customer conduit in CC_1
                AffixSpanEquipmentToContainer(CustomerConduit_CC_1_to_SDU_2, NodeContainer_CC_1, NodeContainerSideEnum.North);


                Thread.Sleep(100);

                _conduitsCreated = true;
            }

            return this;
        }

        private Guid PlaceConduit(Guid specificationId, RouteNetworkElementIdList walkIds, AddressInfo? addressInfo = null)
        {
            // Register walk of interest
            var walkOfInterestId = Guid.NewGuid();
            var registerWalkOfInterestCommand = new RegisterWalkOfInterest(Guid.NewGuid(), new UserContext("test", Guid.Empty), walkOfInterestId, walkIds);
            var registerWalkOfInterestCommandResult = _commandDispatcher.HandleAsync<RegisterWalkOfInterest, Result<RouteNetworkInterest>>(registerWalkOfInterestCommand).Result;

            // Place conduit
            var placeSpanEquipmentCommand = new PlaceSpanEquipmentInRouteNetwork(Guid.NewGuid(), new UserContext("test", Guid.Empty), Guid.NewGuid(), specificationId, registerWalkOfInterestCommandResult.Value)
            {
                AddressInfo = addressInfo
            };

            var placeSpanEquipmentResult =  _commandDispatcher.HandleAsync<PlaceSpanEquipmentInRouteNetwork, Result>(placeSpanEquipmentCommand).Result;

            if (placeSpanEquipmentResult.IsFailed)
                throw new ApplicationException(placeSpanEquipmentResult.Errors.First().Message);

            return placeSpanEquipmentCommand.SpanEquipmentId;
        }

        private Guid PlaceNodeContainer(Guid specificationId, Guid manufacturerId, Guid routeNodeId)
        {
            var nodeOfInterestId = Guid.NewGuid();
            var registerNodeOfInterestCommand = new RegisterNodeOfInterest(Guid.NewGuid(), new UserContext("test", Guid.Empty), nodeOfInterestId, routeNodeId);
            var registerNodeOfInterestCommandResult = _commandDispatcher.HandleAsync<RegisterNodeOfInterest, Result<RouteNetworkInterest>>(registerNodeOfInterestCommand).Result;

            var placeNodeContainerCommand = new PlaceNodeContainerInRouteNetwork(Guid.NewGuid(), new UserContext("test", Guid.Empty), Guid.NewGuid(), specificationId, registerNodeOfInterestCommandResult.Value)
            {
                ManufacturerId = manufacturerId
            };

            var placeNodeContainerResult = _commandDispatcher.HandleAsync<PlaceNodeContainerInRouteNetwork, Result>(placeNodeContainerCommand).Result;

            if (placeNodeContainerResult.IsFailed)
                throw new ApplicationException(placeNodeContainerResult.Errors.First().Message);

            return placeNodeContainerCommand.NodeContainerId;
        }

        private void AffixSpanEquipmentToContainer(Guid spanEquipmentId, Guid nodeContainerId, NodeContainerSideEnum side)
        {
            var affixConduitToContainerCommand = new AffixSpanEquipmentToNodeContainer(Guid.NewGuid(), new UserContext("test", Guid.Empty),
               spanEquipmentOrSegmentId: spanEquipmentId,
               nodeContainerId: nodeContainerId,
               nodeContainerIngoingSide: side
           );

            var affixResult = _commandDispatcher.HandleAsync<AffixSpanEquipmentToNodeContainer, Result>(affixConduitToContainerCommand).Result;

            if (affixResult.IsFailed)
                throw new ApplicationException(affixResult.Errors.First().Message);
        }
    }
}

