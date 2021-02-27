using OpenFTTH.Schematic.API.Model.DiagramLayout;
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

            /*
            // Build ingoing/left label block
            var vestLabelBlock = CreateLabelBlock(_spanEquipmentViewModel.GetInnerSpanLabels(InnerLabelDirectionEnum.Ingoing), BlockSideEnum.Vest);

            result.AddRange(vestLabelBlock.CreateDiagramObjects(diagram, offsetX, offsetY));

            // Build outgoing/right label block
            var eastLabelBlock = CreateLabelBlock(_spanEquipmentViewModel.GetInnerSpanLabels(InnerLabelDirectionEnum.Outgoing), BlockSideEnum.East);

            result.AddRange(eastLabelBlock.CreateDiagramObjects(diagram, offsetX + _spanEquipmentAreaWidth + _labelAreaWidth, offsetY));
            */

            // Build span equipment block
            var spanEquipmentBlock = CreateSpanEquipmentBlock("OuterConduit", _spanEquipmentViewModel.GetInnerSpanDiagramInfos("InnerConduit"));

            result.AddRange(spanEquipmentBlock.CreateDiagramObjects(diagram, offsetX, offsetY));

            return result;
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

        private LineBlock CreateLabelBlock(List<string> labels, BlockSideEnum side)
        {
            var labelBlock = new LineBlock()
            {
                MinWidth = 0.01,
                IsVisible = false,
                Margin = _margin
            };

            var fromPortSide = BlockSideEnum.Vest;
            var toPortSide = BlockSideEnum.East;
            string style = null;

            if (side == BlockSideEnum.Vest)
            {
                fromPortSide = BlockSideEnum.Vest;
                toPortSide = BlockSideEnum.East;
                style = "LabelMediumTextRightAlligned";
            }
            else if (side == BlockSideEnum.East)
            {
                fromPortSide = BlockSideEnum.Vest;
                toPortSide = BlockSideEnum.East;
                style = "LabelMediumTextLeftAlligned";
            }
            else if (side == BlockSideEnum.North)
            {
                fromPortSide = BlockSideEnum.South;
                toPortSide = BlockSideEnum.North;
                style = "LabelMediumTextRightAlligned";
            }
            else if (side == BlockSideEnum.South)
            {
                fromPortSide = BlockSideEnum.South;
                toPortSide = BlockSideEnum.North;
                style = "LabelMediumTextLeftAlligned";
            }

            var fromPort = new BlockPort(fromPortSide) { IsVisible = false };
            labelBlock.AddPort(fromPort);

            var toPort = new BlockPort(toPortSide) { IsVisible = false };
            labelBlock.AddPort(toPort);

            int terminalNo = 1;
            foreach (var label in labels)
            {
                new BlockPortTerminal(fromPort) { IsVisible = false };
                new BlockPortTerminal(toPort) { IsVisible = false };

                labelBlock.AddTerminalConnection(fromPortSide, 1, terminalNo, toPortSide, 1, terminalNo, label, style, LineShapeTypeEnum.Line);
                terminalNo++;
            }

            return labelBlock;
        }
    }
}
