using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.Drawing;
using OpenFTTH.Schematic.Business.Lines;
using System.Collections.Generic;

namespace OpenFTTH.Schematic.Business.SchematicBuilder
{
    /// <summary>
    ///  Diagram creation of a span equipment starting, ending or passing through a route network node or element
    /// </summary>
    public class DetachedSpanEquipmentBuilder
    {
        private readonly DetachedSpanEquipmentViewModel _spanEquipmentViewModel;

        private readonly double _spanEquipmentAreaWidth = 300;
        private readonly double _margin = 5;


        public DetachedSpanEquipmentBuilder(DetachedSpanEquipmentViewModel spanEquipmentViewModel)
        {
            _spanEquipmentViewModel = spanEquipmentViewModel;
        }

        public IEnumerable<DiagramObject> CreateDiagramObjects(Diagram diagram, double offsetX, double offsetY)
        {
            List<DiagramObject> result = new List<DiagramObject>();

            // Build span equipment block
            var spanEquipmentBlock = CreateSpanEquipmentBlock("OuterConduit", _spanEquipmentViewModel.GetInnerSpanDiagramInfos("InnerConduit"));
            result.AddRange(spanEquipmentBlock.CreateDiagramObjects(diagram, offsetX, offsetY));

            // Create label on top of span equipment block
            var spanEquipmentLabel = CreateSpanEquipmentTypeLabel(diagram, 0, spanEquipmentBlock.ActualSize.Height + 5);
            result.Add(spanEquipmentLabel);

            return result;
        }

        private DiagramObject CreateSpanEquipmentTypeLabel(Diagram diagram, double x, double y)
        {
            var labelDiagramObject = new DiagramObject(diagram)
            {
                Style = "SpanEquipmentLabel",
                Label = _spanEquipmentViewModel.GetSpanEquipmentLabel(),
                Geometry = GeometryBuilder.Point(x, y)
            };

            return labelDiagramObject;
        }

        private LineBlock CreateSpanEquipmentBlock(string spanStyle, List<SpanDiagramInfo> innerSpanData)
        {
            // Create outer conduits
            var rootSpanInfo = _spanEquipmentViewModel.RootSpanDiagramInfo("OuterConduit");

            var spanEquipmentBlock = new LineBlock()
            {
                MinWidth = _spanEquipmentAreaWidth,
                IsVisible = true,
                Style = rootSpanInfo.StyleName,
                Margin = _margin
            };

            spanEquipmentBlock.SetReference(rootSpanInfo.SpanSegmentId, "SpanStructure");

            // Create inner conduits

            var fromPort = new BlockPort(BlockSideEnum.Vest) { IsVisible = false };
            spanEquipmentBlock.AddPort(fromPort);

            var toPort = new BlockPort(BlockSideEnum.East) { IsVisible = false };
            spanEquipmentBlock.AddPort(toPort);

            var vestLabels = _spanEquipmentViewModel.GetInnerSpanLabels(InnerLabelDirectionEnum.Ingoing);
            var eastLabels = _spanEquipmentViewModel.GetInnerSpanLabels(InnerLabelDirectionEnum.Outgoing);

            int terminalNo = 1;
            foreach (var data in innerSpanData)
            {
                new BlockPortTerminal(fromPort) {
                    IsVisible = true,
                    ShapeType = TerminalShapeTypeEnum.Point,
                    Style = "VestTerminalLabel",
                    Label = vestLabels[terminalNo - 1]
                };

                new BlockPortTerminal(toPort) { 
                    IsVisible = true,
                    ShapeType = TerminalShapeTypeEnum.Point,
                    Style = "EastTerminalLabel",
                    Label = eastLabels[terminalNo - 1]
                };

            var terminalConnection = spanEquipmentBlock.AddTerminalConnection(BlockSideEnum.Vest, 1, terminalNo, BlockSideEnum.East, 1, terminalNo, null, data.StyleName, LineShapeTypeEnum.Polygon);
                terminalConnection.SetReference(data.SpanSegmentId, "SpanStructure");
                terminalNo++;
            }

            return spanEquipmentBlock;
        }
    }
}
