using FluentAssertions;
using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.IO;
using OpenFTTH.Schematic.Business.Lines;
using System.Linq;
using Xunit;

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
            AddTerminalsToPort(vestPort1, 3);

            // East
            var eastPort1 = new BlockPort(BlockSideEnum.East) { IsVisible = false };
            lineBlock.AddPort(eastPort1);
            AddTerminalsToPort(eastPort1, 3);

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
        public void LabelTest2()
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
            AddTerminalsToPort(vestPort1, 3);

            var vestPort2 = new BlockPort(BlockSideEnum.West);
            lineBlock.AddPort(vestPort2);
            AddTerminalsToPort(vestPort2, 3);

            // East
            var eastPort1 = new BlockPort(BlockSideEnum.East);
            lineBlock.AddPort(eastPort1);
            AddTerminalsToPort(eastPort1, 3);

            var eastPort2 = new BlockPort(BlockSideEnum.East);
            lineBlock.AddPort(eastPort2);
            AddTerminalsToPort(eastPort2, 3);


            // North
            var northPort1 = new BlockPort(BlockSideEnum.North);
            lineBlock.AddPort(northPort1);
            AddTerminalsToPort(northPort1, 3);

            var northPort2 = new BlockPort(BlockSideEnum.North);
            lineBlock.AddPort(northPort2);
            AddTerminalsToPort(northPort2, 3);

            lineBlock.SetSideCenterAlignment(BlockSideEnum.North);


            // South
            var southPort1 = new BlockPort(BlockSideEnum.South, "SouthPortStyle1", "SouthPortLabel1");
            lineBlock.AddPort(southPort1);
            AddTerminalsToPort(southPort1, 3);

            var southPort2 = new BlockPort(BlockSideEnum.South, "SouthPortStyle2", "SouthPortLabel2");
            lineBlock.AddPort(southPort2);
            AddTerminalsToPort(southPort2, 3);


            //lineBlock.AddPortConnection(BlockSideEnum.Vest, 1, BlockSideEnum.East, 2);
            lineBlock.AddTerminalConnection(BlockSideEnum.West, 1, 1, BlockSideEnum.North, 2, 1, "Hest", "Cable", LineShapeTypeEnum.Polygon);

            var diagramObjects = lineBlock.CreateDiagramObjects(diagram, 0, 0);

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");
        }

        [Fact]
        public void AllDirectionsTerminalLineDrawingTest_ShouldSucceed()
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
            var vestPort = new BlockPort(BlockSideEnum.West) { IsVisible = false };
            lineBlock.AddPort(vestPort);
            AddTerminalsToPort(vestPort, 4);

            // North
            var northPort = new BlockPort(BlockSideEnum.North) { IsVisible = false };
            lineBlock.AddPort(northPort);
            AddTerminalsToPort(northPort, 4);

            // East
            var eastPort = new BlockPort(BlockSideEnum.East) { IsVisible = false };
            lineBlock.AddPort(eastPort);
            AddTerminalsToPort(eastPort, 4);

            // South
            var southPort = new BlockPort(BlockSideEnum.South) { IsVisible = false };
            lineBlock.AddPort(southPort);
            AddTerminalsToPort(southPort, 4);

            // West connections
            lineBlock.AddTerminalConnection(BlockSideEnum.West, 1, 1, BlockSideEnum.North, 1, 1, "This is a label text", "This should be a polyline", LineShapeTypeEnum.Line);
            lineBlock.AddTerminalConnection(BlockSideEnum.West, 1, 2, BlockSideEnum.East, 1, 1, "This is a label text", "This should be a polyline", LineShapeTypeEnum.Line);
            lineBlock.AddTerminalConnection(BlockSideEnum.West, 1, 3, BlockSideEnum.South, 1, 1, "This is a label text", "This should be a polyline", LineShapeTypeEnum.Line);

            // North connections
            lineBlock.AddTerminalConnection(BlockSideEnum.North, 1, 2, BlockSideEnum.East, 1, 2, "This is a label text", "This should be a polyline", LineShapeTypeEnum.Line);
            lineBlock.AddTerminalConnection(BlockSideEnum.North, 1, 3, BlockSideEnum.South, 1, 2, "This is a label text", "This should be a polyline", LineShapeTypeEnum.Line);

            // East connections
            lineBlock.AddTerminalConnection(BlockSideEnum.East, 1, 3, BlockSideEnum.South, 1, 3, "This is a label text", "This should be a polyline", LineShapeTypeEnum.Line);

            // Act
            lineBlock.CreateDiagramObjects(diagram, 0, 0);

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");

            // Assert
            diagram.DiagramObjects.Count(d => d.Style == "This should be a polyline").Should().Be(6);

        }

        [Fact]
        public void SouthEastPostDrawingTest_ShouldSucceed()
        {
            Diagram diagram = new Diagram();

            var lineBlock = new LineBlock()
            {
                MinHeight = 200,
                MinWidth = 300,
                Margin = 20,
                IsVisible = false
            };

            // East
            var eastPort = new BlockPort(BlockSideEnum.East) { IsVisible = false };
            lineBlock.AddPort(eastPort);
            AddTerminalsToPort(eastPort, 4);

            // South
            var southPort = new BlockPort(BlockSideEnum.South) { IsVisible = false };
            lineBlock.AddPort(southPort);
            AddTerminalsToPort(southPort, 4);

            // East South connections
            lineBlock.AddTerminalConnection(BlockSideEnum.South, 1, 1, BlockSideEnum.East, 1, 1, "This is a label text", "This should be a polyline", LineShapeTypeEnum.Polygon);

            lineBlock.CreateDiagramObjects(diagram, 0, 0);

            if (System.Environment.OSVersion.Platform.ToString() == "Win32NT")
                new GeoJsonExporter(diagram).Export("c:/temp/diagram/test.geojson");
        }

        private void AddTerminalsToPort(BlockPort port, int nTerminals)
        {
            for (int i = 0; i < nTerminals; i++)
            {
                new BlockPortTerminal(port) { IsVisible = false };
            }
        }
    }
}
