using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.IO;
using OpenFTTH.Schematic.Business.Lines;
using System;
using Xunit;

namespace OpenFTTH.Schematic.Tests.LineBlocks
{
    public class LineBlockBasicTests
    {
        [Fact]
        public void Test1()
        {
            Diagram diagram = new Diagram();

            var lineBlock = new LineBlock(0,0);

            lineBlock.AddPort(new BlockPort(BlockSideEnum.Vest));

            lineBlock.AddPort(new BlockPort(BlockSideEnum.East));

            lineBlock.AddPortConnection(BlockSideEnum.Vest, 1, BlockSideEnum.East, 1);

            var diagramObjects = lineBlock.CreateDiagramObjects(diagram, 0, 0);

            new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");
        }
    }
}
