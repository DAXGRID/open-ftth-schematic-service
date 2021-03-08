using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.Drawing;
using OpenFTTH.Schematic.Business.Layout;
using OpenFTTH.Schematic.Business.Lines;
using System.Collections.Generic;

namespace OpenFTTH.Schematic.Business.SchematicBuilder
{
    /// <summary>
    ///  Diagram creation of a span equipment starting, ending or passing through a route network node or element
    /// </summary>
    public class DetachedSpanEquipmentBuilder
    {
        private readonly SpanEquipmentViewModel _spanEquipmentViewModel;

        private readonly double _spanEquipmentAreaWidth = 300;
        private readonly double _spanEquipmentBlockMargin = 5;
        private readonly double _spanEquipmentLabelOffset = 5;


        public DetachedSpanEquipmentBuilder(SpanEquipmentViewModel spanEquipmentViewModel)
        {
            _spanEquipmentViewModel = spanEquipmentViewModel;
        }

        public Size CreateDiagramObjects(Diagram diagram, double offsetX, double offsetY)
        {
            List<DiagramObject> result = new List<DiagramObject>();

            // Build span equipment block
            var spanEquipmentBlock = CreateSpanEquipmentBlock();
            result.AddRange(spanEquipmentBlock.CreateDiagramObjects(diagram, offsetX, offsetY));

            // Create label on top of span equipment block
            var spanEquipmentLabel = CreateSpanEquipmentTypeLabel(diagram, offsetX, offsetY + spanEquipmentBlock.ActualSize.Height + _spanEquipmentLabelOffset);
            result.Add(spanEquipmentLabel);

            return new Size(spanEquipmentBlock.ActualSize.Height + _spanEquipmentLabelOffset, spanEquipmentBlock.ActualSize.Width);
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

        private LineBlock CreateSpanEquipmentBlock()
        {
            if (_spanEquipmentViewModel.IsPassThrough)
                return CreateConduitPassThroughBlock();
            else
                return CreateConduitEndBlock();
        }

        private LineBlock CreateConduitPassThroughBlock()
        {
            // Create outer conduits
            var rootSpanInfo = _spanEquipmentViewModel.RootSpanDiagramInfo("OuterConduit");

            var spanEquipmentBlock = new LineBlock()
            {
                MinWidth = _spanEquipmentAreaWidth,
                IsVisible = true,
                Style = rootSpanInfo.StyleName,
                Margin = _spanEquipmentBlockMargin
            };

            spanEquipmentBlock.SetReference(rootSpanInfo.SpanSegmentId, "SpanSegment");

            // Create inner conduits
            var innerSpanData = _spanEquipmentViewModel.GetInnerSpanDiagramInfos("InnerConduit");

            var fromPort = new BlockPort(BlockSideEnum.West) { IsVisible = false };
            spanEquipmentBlock.AddPort(fromPort);

            var toPort = new BlockPort(BlockSideEnum.East) { IsVisible = false };
            spanEquipmentBlock.AddPort(toPort);

            var vestLabels = _spanEquipmentViewModel.GetInnerSpanLabels(InnerLabelDirectionEnum.Ingoing);
            var eastLabels = _spanEquipmentViewModel.GetInnerSpanLabels(InnerLabelDirectionEnum.Outgoing);

            int terminalNo = 1;
            foreach (var data in innerSpanData)
            {
                var fromTerminal = new BlockPortTerminal(fromPort)
                {
                    IsVisible = true,
                    ShapeType = TerminalShapeTypeEnum.Point,
                    PointStyle = "WestTerminalLabel",
                    PointLabel = vestLabels[terminalNo - 1]
                };

                fromTerminal.SetReference(data.SpanSegmentId, "SpanSegment");

                var toTerminal= new BlockPortTerminal(toPort)
                {
                    IsVisible = true,
                    ShapeType = TerminalShapeTypeEnum.Point,
                    PointStyle = "EastTerminalLabel",
                    PointLabel = eastLabels[terminalNo - 1]
                };

                toTerminal.SetReference(data.SpanSegmentId, "SpanSegment");

                var terminalConnection = spanEquipmentBlock.AddTerminalConnection(BlockSideEnum.West, 1, terminalNo, BlockSideEnum.East, 1, terminalNo, null, data.StyleName, LineShapeTypeEnum.Polygon);
                terminalConnection.SetReference(data.SpanSegmentId, "SpanSegment");
                terminalNo++;
            }

            return spanEquipmentBlock;
        }

        private LineBlock CreateConduitEndBlock()
        {
            // Create outer conduits
            var rootSpanInfo = _spanEquipmentViewModel.RootSpanDiagramInfo("OuterConduit");

            var spanEquipmentBlock = new LineBlock()
            {
                MinWidth = _spanEquipmentAreaWidth / 2,
                IsVisible = true,
                Style = rootSpanInfo.StyleName,
                Margin = _spanEquipmentBlockMargin
            };

            spanEquipmentBlock.SetReference(rootSpanInfo.SpanSegmentId, "SpanSegment");

            // Create inner conduits
            var innerSpanData = _spanEquipmentViewModel.GetInnerSpanDiagramInfos("InnerConduit");

            var fromPort = new BlockPort(BlockSideEnum.West) { IsVisible = false };
            spanEquipmentBlock.AddPort(fromPort);

            var toPort = new BlockPort(BlockSideEnum.East) { IsVisible = false };
            spanEquipmentBlock.AddPort(toPort);

            var labels = _spanEquipmentViewModel.GetInnerSpanLabels(InnerLabelDirectionEnum.FromOppositeEndOfNode);

            int terminalNo = 1;
            foreach (var data in innerSpanData)
            {
                var fromTerminal = new BlockPortTerminal(fromPort)
                {
                    IsVisible = true,
                    ShapeType = TerminalShapeTypeEnum.Point,
                    PointStyle = "WestTerminalLabel",
                    PointLabel = labels[terminalNo - 1]
                };

                fromTerminal.SetReference(data.SpanSegmentId, "SpanSegment");

                new BlockPortTerminal(toPort)
                {
                    IsVisible = true,
                    ShapeType = TerminalShapeTypeEnum.None,
                };

                var terminalConnection = spanEquipmentBlock.AddTerminalConnection(BlockSideEnum.West, 1, terminalNo, BlockSideEnum.East, 1, terminalNo, null, data.StyleName, LineShapeTypeEnum.Polygon);
                terminalConnection.SetReference(data.SpanSegmentId, "SpanSegment");
                terminalNo++;
            }

            return spanEquipmentBlock;
        }
    }
}
