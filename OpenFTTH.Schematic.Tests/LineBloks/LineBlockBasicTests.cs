using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.IO;
using OpenFTTH.Schematic.Business.Lines;
using Xunit;
using FluentAssertions;
using System.Linq;

namespace OpenFTTH.Schematic.Tests.LineBlocks
{
    public class LineBlockBasicTests
    {
        [Fact]
        public void LineBlockWithLabeledLinesTest()
        {
            Diagram diagram = new Diagram();

            var lineBlock = new LineBlock()
            {
                MinHeight = 200,
                MinWidth = 300,
                Margin = 20,
                IsVisible = false
            };

            // Vest
            var vestPort1 = new BlockPort(BlockSideEnum.West) { IsVisible = false };
            lineBlock.AddPort(vestPort1);
            AddThreeTerminalsToPort(vestPort1);

            // East
            var eastPort1 = new BlockPort(BlockSideEnum.East) { IsVisible = false };
            lineBlock.AddPort(eastPort1);
            AddThreeTerminalsToPort(eastPort1);

            lineBlock.AddTerminalConnection(BlockSideEnum.West, 1, 1, BlockSideEnum.East, 1, 1, "This is a label text", "This should be a polyline", LineShapeTypeEnum.Line);

            // Act
            lineBlock.CreateDiagramObjects(diagram, 0, 0);

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            diagram.DiagramObjects.Count.Should().Be(1);
            diagram.DiagramObjects.First().Geometry.IsValid.Should().BeTrue();
            diagram.DiagramObjects.First().Geometry.Length.Should().Be(0.03);
            diagram.DiagramObjects.First().Label.Should().Be("This is a label text");
            diagram.DiagramObjects.First().Style.Should().Be("This should be a polyline");
        }


        [Fact]
        public void test2()
        { 
            Diagram diagram = new Diagram();

            var lineBlock = new LineBlock()
            {
                MinHeight = 200,
                MinWidth = 300,
                Margin = 20
            };

            
            // Vest
            var vestPort1 = new BlockPort(BlockSideEnum.West);
            lineBlock.AddPort(vestPort1);
            AddThreeTerminalsToPort(vestPort1);

            var vestPort2 = new BlockPort(BlockSideEnum.West);
            lineBlock.AddPort(vestPort2);
            AddThreeTerminalsToPort(vestPort2);

            // East
            var eastPort1 = new BlockPort(BlockSideEnum.East);
            lineBlock.AddPort(eastPort1);
            AddThreeTerminalsToPort(eastPort1);

            var eastPort2 = new BlockPort(BlockSideEnum.East);
            lineBlock.AddPort(eastPort2);
            AddThreeTerminalsToPort(eastPort2);


            // North
            var northPort1 = new BlockPort(BlockSideEnum.North);
            lineBlock.AddPort(northPort1);
            AddThreeTerminalsToPort(northPort1);

            var northPort2 = new BlockPort(BlockSideEnum.North);
            lineBlock.AddPort(northPort2);
            AddThreeTerminalsToPort(northPort2);

            lineBlock.SetSideCenterAlignment(BlockSideEnum.North);


            // South
            var southPort1 = new BlockPort(BlockSideEnum.South, "SouthPortStyle1", "SouthPortLabel1");
            lineBlock.AddPort(southPort1);
            AddThreeTerminalsToPort(southPort1);

            var southPort2 = new BlockPort(BlockSideEnum.South, "SouthPortStyle2", "SouthPortLabel2");
            lineBlock.AddPort(southPort2);
            AddThreeTerminalsToPort(southPort2);


            //lineBlock.AddPortConnection(BlockSideEnum.Vest, 1, BlockSideEnum.East, 2);
            lineBlock.AddTerminalConnection(BlockSideEnum.West, 1, 1, BlockSideEnum.North, 2, 1, "Hest", "Cable", LineShapeTypeEnum.Polygon);

            var diagramObjects = lineBlock.CreateDiagramObjects(diagram, 0, 0);

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");
        }

        private void AddThreeTerminalsToPort(BlockPort port)
        {
            new BlockPortTerminal(port) { IsVisible = false };
            new BlockPortTerminal(port) { IsVisible = false };
            new BlockPortTerminal(port) { IsVisible = false };
        }
    }
}
