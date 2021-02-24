using FluentResults;
using OpenFTTH.CQRS;
using OpenFTTH.RouteNetwork.API.Commands;
using OpenFTTH.RouteNetwork.API.Model;
using OpenFTTH.TestData;
using OpenFTTH.UtilityGraphService.API.Commands;
using System;
using System.Linq;

namespace OpenFTTH.TestData
{
    public class TestConduits
    {
        private ICommandDispatcher _commandDispatcher;
        private IQueryDispatcher _queryDispatcher;
        private TestConduitSpecifications _conduitSpecs;

        public Guid MultiConduit_5x10_HH_1_to_HH_10;
        public Guid MultiConduit_10x10_HH_1_to_HH_10;
        public Guid FlexConduit_40_Red_HH_2_to_FP_2;
        public Guid FlexConduit_40_Red_CC_1_to_SP_1;

        public TestConduits(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _conduitSpecs = new TestConduitSpecifications(commandDispatcher, queryDispatcher);
        }

        public TestConduits Run()
        {
            // Place some conduits in the route network we can play with
            MultiConduit_5x10_HH_1_to_HH_10 = PlaceConduit(_conduitSpecs.Multi_Ø40_5x10, new RouteNetworkElementIdList() { TestRouteNetwork.S2, TestRouteNetwork.S4, TestRouteNetwork.S13 });
            MultiConduit_10x10_HH_1_to_HH_10 = PlaceConduit(_conduitSpecs.Multi_Ø50_10x10, new RouteNetworkElementIdList() { TestRouteNetwork.S2, TestRouteNetwork.S4, TestRouteNetwork.S13 });
            FlexConduit_40_Red_HH_2_to_FP_2 = PlaceConduit(_conduitSpecs.Flex_Ø40_Red, new RouteNetworkElementIdList() { TestRouteNetwork.S3 });
            FlexConduit_40_Red_CC_1_to_SP_1 = PlaceConduit(_conduitSpecs.Flex_Ø40_Red, new RouteNetworkElementIdList() { TestRouteNetwork.S5 });

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
    }
}

