using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.Drawing;
using OpenFTTH.Schematic.Business.Layout;
using OpenFTTH.Schematic.Business.Lines;
using System.Collections.Generic;
using System.Linq;

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
                Geometry = GeometryBuilder.Point(x, y),
                DrawingOrder = 1000
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
                Margin = _spanEquipmentBlockMargin,
                DrawingOrder = 400
            };

            spanEquipmentBlock.SetReference(rootSpanInfo.IngoingSegmentId, "SpanSegment");

            // Create inner conduits
            var innerSpanData = _spanEquipmentViewModel.GetInnerSpanDiagramInfos("InnerConduit");

            var fromPort = new BlockPort(BlockSideEnum.West) { IsVisible = false, DrawingOrder = 420 };
            spanEquipmentBlock.AddPort(fromPort);

            var toPort = new BlockPort(BlockSideEnum.East) { IsVisible = false, DrawingOrder = 420 };
            spanEquipmentBlock.AddPort(toPort);

            int terminalNo = 1;

            var orderedinnerSpanData = innerSpanData.OrderBy(i => (1000 - i.Position));

            bool innerSpansFound = false;

            foreach (var spanInfo in orderedinnerSpanData)
            {
                var fromTerminal = new BlockPortTerminal(fromPort)
                {
                    IsVisible = true,
                    ShapeType = TerminalShapeTypeEnum.Point,
                    PointStyle = "WestTerminalLabel",
                    PointLabel = spanInfo.IngoingRouteNodeName,
                    DrawingOrder = 520
                };

                fromTerminal.SetReference(spanInfo.IngoingSegmentId, "SpanSegment");

                var toTerminal= new BlockPortTerminal(toPort)
                {
                    IsVisible = true,
                    ShapeType = TerminalShapeTypeEnum.Point,
                    PointStyle = "EastTerminalLabel",
                    PointLabel = spanInfo.OutgoingRouteNodeName,
                    DrawingOrder = 520
                };

                toTerminal.SetReference(spanInfo.OutgoingSegmentId, "SpanSegment");

                var terminalConnection = spanEquipmentBlock.AddTerminalConnection(BlockSideEnum.West, 1, terminalNo, BlockSideEnum.East, 1, terminalNo, null, spanInfo.StyleName, LineShapeTypeEnum.Polygon);
                terminalConnection.DrawingOrder = 510;
                terminalConnection.SetReference(spanInfo.IngoingSegmentId, "SpanSegment");
                terminalNo++;

                innerSpansFound = true;
            }

            if (!innerSpansFound)
            {
                var fromTerminal = new BlockPortTerminal(fromPort)
                {
                    IsVisible = true,
                    ShapeType = TerminalShapeTypeEnum.Point,
                    PointStyle = "WestTerminalLabel",
                    PointLabel = rootSpanInfo.IngoingRouteNodeName,
                    DrawingOrder = 520
                };

                fromTerminal.SetReference(rootSpanInfo.IngoingSegmentId, "SpanSegment");

                var toTerminal = new BlockPortTerminal(toPort)
                {
                    IsVisible = true,
                    ShapeType = TerminalShapeTypeEnum.Point,
                    PointStyle = "EastTerminalLabel",
                    PointLabel = rootSpanInfo.OutgoingRouteNodeName,
                    DrawingOrder = 520
                };

                toTerminal.SetReference(rootSpanInfo.OutgoingSegmentId, "SpanSegment");

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
                Margin = _spanEquipmentBlockMargin,
                DrawingOrder = 400
            };

            spanEquipmentBlock.SetReference(rootSpanInfo.SegmentId, "SpanSegment");

            // Create inner conduits
            var innerSpanData = _spanEquipmentViewModel.GetInnerSpanDiagramInfos("InnerConduit");

            var fromPort = new BlockPort(BlockSideEnum.West) { IsVisible = false, DrawingOrder = 420 };
            spanEquipmentBlock.AddPort(fromPort);

            var toPort = new BlockPort(BlockSideEnum.East) { IsVisible = false, DrawingOrder = 420 };
            spanEquipmentBlock.AddPort(toPort);

            int terminalNo = 1;

            var orderedinnerSpanData = innerSpanData.OrderBy(i => (1000 - i.Position));

            bool innerSpansFound = false;

            foreach (var data in orderedinnerSpanData)
            {
                var fromTerminal = new BlockPortTerminal(fromPort)
                {
                    IsVisible = true,
                    ShapeType = TerminalShapeTypeEnum.Point,
                    PointStyle = "WestTerminalLabel",
                    PointLabel = data.OppositeRouteNodeName,
                    DrawingOrder = 520
                };

                fromTerminal.SetReference(data.SegmentId, "SpanSegment");

                new BlockPortTerminal(toPort)
                {
                    IsVisible = true,
                    ShapeType = TerminalShapeTypeEnum.None,
                    DrawingOrder = 520
                };

                var terminalConnection = spanEquipmentBlock.AddTerminalConnection(BlockSideEnum.West, 1, terminalNo, BlockSideEnum.East, 1, terminalNo, null, data.StyleName, LineShapeTypeEnum.Polygon);
                terminalConnection.DrawingOrder = 510;
                terminalConnection.SetReference(data.SegmentId, "SpanSegment");
                terminalNo++;

                innerSpansFound = true;
            }

            if (!innerSpansFound)
            {
                var fromTerminal = new BlockPortTerminal(fromPort)
                {
                    IsVisible = true,
                    ShapeType = TerminalShapeTypeEnum.Point,
                    PointStyle = "WestTerminalLabel",
                    PointLabel = rootSpanInfo.OppositeRouteNodeName,
                    DrawingOrder = 520
                };

                fromTerminal.SetReference(rootSpanInfo.IngoingSegmentId, "SpanSegment");
               
                terminalNo++;
            }

            return spanEquipmentBlock;
        }
    }
}
