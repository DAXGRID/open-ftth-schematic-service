using FluentResults;
using OpenFTTH.CQRS;
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

        public static Guid MultiConduit_5x10_HH_1_to_HH_10;
        public static Guid MultiConduit_10x10_HH_1_to_HH_10;
        public static Guid FlexConduit_40_Red_HH_2_to_FP_2;
        public static Guid FlexConduit_40_Red_CC_1_to_SP_1;
        public static Guid MultiConduit_3x10_CC_1_to_SP_1;
        public static Guid MultiConduit_3x10_CC_1_to_HH_11;
        public static Guid FlexConduit_40_Red_SDU_1_to_SDU_2;
        public static Guid MultiConduit_5x10_SDU_1_to_SDU_2;

        public static Guid NodeContainer_HH_1;
        public static Guid NodeContainer_CC_1;
        public static Guid NodeContainer_J_1;

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
                MultiConduit_5x10_HH_1_to_HH_10 = PlaceConduit(TestSpecifications.Multi_Ø40_5x10, new RouteNetworkElementIdList() { TestRouteNetwork.S2, TestRouteNetwork.S4, TestRouteNetwork.S13 });
                MultiConduit_10x10_HH_1_to_HH_10 = PlaceConduit(TestSpecifications.Multi_Ø50_10x10, new RouteNetworkElementIdList() { TestRouteNetwork.S2, TestRouteNetwork.S4, TestRouteNetwork.S13 });
                FlexConduit_40_Red_HH_2_to_FP_2 = PlaceConduit(TestSpecifications.Flex_Ø40_Red, new RouteNetworkElementIdList() { TestRouteNetwork.S3 });
                FlexConduit_40_Red_CC_1_to_SP_1 = PlaceConduit(TestSpecifications.Flex_Ø40_Red, new RouteNetworkElementIdList() { TestRouteNetwork.S5 });
                MultiConduit_3x10_CC_1_to_SP_1 = PlaceConduit(TestSpecifications.Multi_Ø32_3x10, new RouteNetworkElementIdList() { TestRouteNetwork.S5 });
                MultiConduit_3x10_CC_1_to_HH_11 = PlaceConduit(TestSpecifications.Multi_Ø32_3x10, new RouteNetworkElementIdList() { TestRouteNetwork.S5, TestRouteNetwork.S6, TestRouteNetwork.S9, TestRouteNetwork.S11 });
                FlexConduit_40_Red_SDU_1_to_SDU_2 = PlaceConduit(TestSpecifications.Flex_Ø40_Red, new RouteNetworkElementIdList() { TestRouteNetwork.S7, TestRouteNetwork.S8 });
                MultiConduit_5x10_SDU_1_to_SDU_2 = PlaceConduit(TestSpecifications.Multi_Ø40_5x10, new RouteNetworkElementIdList() { TestRouteNetwork.S7, TestRouteNetwork.S8 });


                // Place node containers
                NodeContainer_HH_1 = PlaceNodeContainer(TestSpecifications.Well_Fiberpowertech_37_EK_378_400x800, TestSpecifications.Manu_Fiberpowertech, TestRouteNetwork.HH_1);
                NodeContainer_CC_1 = PlaceNodeContainer(TestSpecifications.Conduit_Closure_Emtelle_Branch_Box, TestSpecifications.Manu_Emtelle, TestRouteNetwork.CC_1);
                NodeContainer_J_1 = PlaceNodeContainer(TestSpecifications.Conduit_Closure_Emtelle_Branch_Box, TestSpecifications.Manu_Emtelle, TestRouteNetwork.J_1);

                // Affix 5x10 and 3x10 in CC 1
                AffixSpanEquipmentToContainer(MultiConduit_5x10_HH_1_to_HH_10, NodeContainer_CC_1, NodeContainerSideEnum.West);
                AffixSpanEquipmentToContainer(MultiConduit_3x10_CC_1_to_SP_1, NodeContainer_CC_1, NodeContainerSideEnum.North);

                // Affix 3x10 in J_1
                AffixSpanEquipmentToContainer(MultiConduit_3x10_CC_1_to_HH_11, NodeContainer_J_1, NodeContainerSideEnum.West);

                // Affix flex conduit in J_1
                AffixSpanEquipmentToContainer(FlexConduit_40_Red_SDU_1_to_SDU_2, NodeContainer_J_1, NodeContainerSideEnum.West);

                Thread.Sleep(100);

                _conduitsCreated = true;
            }

            return this;
        }

        private Guid PlaceConduit(Guid specificationId, RouteNetworkElementIdList walkIds)
        {
            // Register walk of interest
            var walkOfInterestId = Guid.NewGuid();
            var registerWalkOfInterestCommand = new RegisterWalkOfInterest(walkOfInterestId, walkIds);
            var registerWalkOfInterestCommandResult = _commandDispatcher.HandleAsync<RegisterWalkOfInterest, Result<RouteNetworkInterest>>(registerWalkOfInterestCommand).Result;

            // Place conduit
            var placeSpanEquipmentCommand = new PlaceSpanEquipmentInRouteNetwork(Guid.NewGuid(), specificationId, registerWalkOfInterestCommandResult.Value);
            var placeSpanEquipmentResult =  _commandDispatcher.HandleAsync<PlaceSpanEquipmentInRouteNetwork, Result>(placeSpanEquipmentCommand).Result;

            if (placeSpanEquipmentResult.IsFailed)
                throw new ApplicationException(placeSpanEquipmentResult.Errors.First().Message);

            return placeSpanEquipmentCommand.SpanEquipmentId;
        }

        private Guid PlaceNodeContainer(Guid specificationId, Guid manufacturerId, Guid routeNodeId)
        {
            var nodeOfInterestId = Guid.NewGuid();
            var registerNodeOfInterestCommand = new RegisterNodeOfInterest(nodeOfInterestId, routeNodeId);
            var registerNodeOfInterestCommandResult = _commandDispatcher.HandleAsync<RegisterNodeOfInterest, Result<RouteNetworkInterest>>(registerNodeOfInterestCommand).Result;

            var placeNodeContainerCommand = new PlaceNodeContainerInRouteNetwork(Guid.NewGuid(), specificationId, registerNodeOfInterestCommandResult.Value)
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
            var affixConduitToContainerCommand = new AffixSpanEquipmentToNodeContainer(
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

