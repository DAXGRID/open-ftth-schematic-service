using OpenFTTH.CQRS;
using OpenFTTH.TestData;
using Xunit;

namespace OpenFTTH.Schematic.Tests.NodeSchematic
{
    public class NodeSchematicTests
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        private static bool _testDataAlreadyAdded;
        private static TestConduitSpecifications _conduitSpecs;
        private static TestConduits _conduits;

        public NodeSchematicTests(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;

            _conduitSpecs = new TestConduitSpecifications(commandDispatcher, queryDispatcher).Run();
            _conduits = new TestConduits(commandDispatcher, queryDispatcher).Run();
        }

        [Fact]
        public async void Test()
        {

        }
    }
}
