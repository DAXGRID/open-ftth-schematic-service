using NetTopologySuite.Geometries;
using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.Drawing;
using OpenFTTH.Schematic.Business.Layout;
using OpenFTTH.Schematic.Business.Lines;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenFTTH.Schematic.Business.SchematicBuilder
{
    /// <summary>
    ///  Diagram creation of a node container - i.e. conduit closures, wells etc.
    /// </summary>
    public class NodeContainerBuilder
    {
        private readonly NodeContainerViewModel _viewModel;

        private readonly double _areaWidth = 300;
        private readonly double _nodeContainerBlockMargin = 60;
        private readonly double _portMargin = 10;
        private readonly double _typeLabelOffset = 5;


        public NodeContainerBuilder(NodeContainerViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public Size CreateDiagramObjects(Diagram diagram, double offsetX, double offsetY)
        {
            List<DiagramObject> result = new List<DiagramObject>();

            // Build node equipment block
            var nodeEquipmentBlock = CreateNodeEquipmentBlock();
            result.AddRange(nodeEquipmentBlock.CreateDiagramObjects(diagram, offsetX, offsetY));

            // Create label on top of span equipment block
            var nodeEquipmentTypeLabel = CreateTypeLabel(diagram, offsetX, offsetY + nodeEquipmentBlock.ActualSize.Height + _typeLabelOffset);
            result.Add(nodeEquipmentTypeLabel);

            // Create the 4 sides
            result.Add(CreateSide(diagram, BlockSideEnum.West, offsetX, offsetY, nodeEquipmentBlock.ActualSize.Width, nodeEquipmentBlock.ActualSize.Height));
            result.Add(CreateSide(diagram, BlockSideEnum.North, offsetX, offsetY, nodeEquipmentBlock.ActualSize.Width, nodeEquipmentBlock.ActualSize.Height));
            result.Add(CreateSide(diagram, BlockSideEnum.East, offsetX, offsetY, nodeEquipmentBlock.ActualSize.Width, nodeEquipmentBlock.ActualSize.Height));
            result.Add(CreateSide(diagram, BlockSideEnum.South, offsetX, offsetY, nodeEquipmentBlock.ActualSize.Width, nodeEquipmentBlock.ActualSize.Height));

            return new Size(nodeEquipmentBlock.ActualSize.Height + _typeLabelOffset, nodeEquipmentBlock.ActualSize.Width);
        }

        private DiagramObject CreateTypeLabel(Diagram diagram, double x, double y)
        {
            var labelDiagramObject = new DiagramObject(diagram)
            {
                Style = "NodeContainerLabel",
                Label = _viewModel.GetNodeContainerTypeLabel(),
                Geometry = GeometryBuilder.Point(x, y)
            };

            return labelDiagramObject;
        }

        private DiagramObject CreateSide(Diagram diagram, BlockSideEnum side, double x, double y, double containerWidth, double containerHeight)
        {
            Geometry lineGeometry = null;

            if (side == BlockSideEnum.West)
            {
                lineGeometry = GeometryBuilder.StraightLine(x, y, x, y + containerHeight);
            }
            else if (side == BlockSideEnum.North)
            {
                lineGeometry = GeometryBuilder.StraightLine(x, y + containerHeight, x + containerWidth, y + containerHeight);
            }
            else if (side == BlockSideEnum.East)
            {
                lineGeometry = GeometryBuilder.StraightLine(x + containerWidth, y, x + containerWidth, y + containerHeight);
            }
            else if (side == BlockSideEnum.South)
            {
                lineGeometry = GeometryBuilder.StraightLine(x, y, x + containerWidth, y);
            }

            var containerSide = new DiagramObject(diagram)
            {
                Style = "NodeContainerSide" + side.ToString(),
                Geometry = lineGeometry
            };

            containerSide.SetReference(_viewModel.NodeContainer.Id, "NodeContainer");

            return containerSide;
        }


        private LineBlock CreateNodeEquipmentBlock()
        {
            var nodeEquipmentBlock = new LineBlock()
            {
                MinWidth = _areaWidth,
                IsVisible = true,
                Style = "NodeContainer",
                Margin = _nodeContainerBlockMargin
            };

            nodeEquipmentBlock.SetReference(_viewModel.NodeContainer.Id, "NodeContainer");

            AffixConduits(nodeEquipmentBlock);

            return nodeEquipmentBlock;
        }
        private void AffixConduits(LineBlock nodeContainerBlock)
        {
            var attachedSpanEquipments = _viewModel.Data.SpanEquipments.Where(s => s.IsAttachedToNodeContainer(_viewModel.Data));

            foreach (var spanEquipment in attachedSpanEquipments)
            {
                var viewModel = new SpanEquipmentViewModel(_viewModel.Data.RouteNetworkElementId, spanEquipment.Id, _viewModel.Data);
                AffixConduit(nodeContainerBlock, viewModel);
            }
        }

        private void AffixConduit(LineBlock nodeContainerBlock, SpanEquipmentViewModel viewModel)
        {
            if (viewModel.IsPassThrough)
            {
                AffixPassThroughConduit(nodeContainerBlock, viewModel);
            }
            else
            {
                AffixConduitEnd(nodeContainerBlock, viewModel);
            }

        }

        private void AffixPassThroughConduit(LineBlock nodeContainerBlock, SpanEquipmentViewModel viewModel)
        {
            var spanDiagramInfo = viewModel.RootSpanDiagramInfo("OuterConduit");

            BlockSideEnum fromSide = MapFromContainerSide(viewModel.Affix.NodeContainerIngoingSide);
            BlockSideEnum toSide = OppositeSide(fromSide);

            bool portsVisible = false;

            // If cut we want to draw the port polygons, because otherwise the outter conduit cannot be seen on the diagram (due to missing connection between the two sides)
            if (spanDiagramInfo.IsCut)
                portsVisible = true;

            // Create outer conduit as port
            var fromPort = new BlockPort(fromSide)
            {
                IsVisible = portsVisible,
                Margin = _portMargin,
                Style = spanDiagramInfo.StyleName
            };

            fromPort.SetReference(viewModel.RootSpanDiagramInfo("OuterConduit").SpanSegmentId, "SpanSegment");
            nodeContainerBlock.AddPort(fromPort);

            var toPort = new BlockPort(toSide)
            {
                IsVisible = portsVisible,
                Margin = _portMargin,
                Style = spanDiagramInfo.StyleName
            };

            toPort.SetReference(viewModel.RootSpanDiagramInfo("OuterConduit").SpanSegmentId, "SpanSegment");
            nodeContainerBlock.AddPort(toPort);

            if (!spanDiagramInfo.IsCut)
            {
                var portConnection = nodeContainerBlock.AddPortConnection(fromSide, fromPort.Index, toSide, toPort.Index, null, spanDiagramInfo.StyleName);
                portConnection.SetReference(spanDiagramInfo.SpanSegmentId, "SpanSegment");
            }

            // Create inner conduits as terminals
            var ingoingInnerConduitLabels = viewModel.GetInnerSpanLabels(InnerLabelDirectionEnum.Ingoing);
            var outgoingInnerConduitLabels = viewModel.GetInnerSpanLabels(InnerLabelDirectionEnum.Outgoing);

            int terminalNo = 1;
            foreach (var data in viewModel.GetInnerSpanDiagramInfos("InnerConduit"))
            {
                TerminalShapeTypeEnum terminalShapeType = TerminalShapeTypeEnum.Point;

                // If cut we want to draw the terminal polygon, because otherwise the conduit cannot be seen on the diagram (due to missing connection between the two sides)
                if (data.IsCut)
                    terminalShapeType = TerminalShapeTypeEnum.PointAndPolygon;

                var fromTerminal = new BlockPortTerminal(fromPort)
                {
                    IsVisible = true,
                    ShapeType = terminalShapeType,
                    PointStyle = fromSide.ToString() + "TerminalLabel",
                    PointLabel = ingoingInnerConduitLabels[terminalNo - 1],
                    PolygonStyle = data.StyleName
                };

                fromTerminal.SetReference(data.SpanSegmentId, "SpanSegment");

                var toTerminal = new BlockPortTerminal(toPort)
                {
                    IsVisible = true,
                    ShapeType = terminalShapeType,
                    PointStyle = toSide.ToString() + "TerminalLabel",
                    PointLabel = outgoingInnerConduitLabels[terminalNo - 1],
                    PolygonStyle = data.StyleName
                };

                toTerminal.SetReference(data.SpanSegmentId, "SpanSegment");

                // Connect the two sides, if the inner conduit is not cut
                if (!data.IsCut)
                {
                    var terminalConnection = nodeContainerBlock.AddTerminalConnection(fromSide, fromPort.Index, terminalNo, toSide, toPort.Index, terminalNo, null, data.StyleName, LineShapeTypeEnum.Polygon);
                    terminalConnection.SetReference(data.SpanSegmentId, "SpanSegment");
                    terminalNo++;
                }
            }
        }

        private void AffixConduitEnd(LineBlock nodeContainerBlock, SpanEquipmentViewModel viewModel)
        {
            BlockSideEnum side = MapFromContainerSide(viewModel.Affix.NodeContainerIngoingSide);

            // Port
            var port = new BlockPort(side)
            {
                IsVisible = true,
                Margin = _portMargin,
                Style = viewModel.RootSpanDiagramInfo("OuterConduit").StyleName
            };

            port.SetReference(viewModel.RootSpanDiagramInfo("OuterConduit").SpanSegmentId, "SpanSegment");

            nodeContainerBlock.AddPort(port);


            // Create inner conduits as terminals
            var innerSpanLabels = viewModel.GetInnerSpanLabels(InnerLabelDirectionEnum.FromOppositeEndOfNode);
            
            int terminalNo = 1;
            foreach (var data in viewModel.GetInnerSpanDiagramInfos("InnerConduit"))
            {
                var terminal = new BlockPortTerminal(port)
                {
                    IsVisible = true,
                    ShapeType = TerminalShapeTypeEnum.PointAndPolygon,
                    PointStyle = side.ToString() + "TerminalLabel",
                    PointLabel = innerSpanLabels[terminalNo - 1],
                    PolygonStyle = data.StyleName
                };

                terminal.SetReference(data.SpanSegmentId, "SpanSegment");
            }
        }

        private static BlockSideEnum OppositeSide(BlockSideEnum fromSide)
        {
            if (fromSide == BlockSideEnum.West)
                return BlockSideEnum.East;
            else if (fromSide == BlockSideEnum.North)
                return BlockSideEnum.South;
            else if (fromSide == BlockSideEnum.East)
                return BlockSideEnum.West;
            else
                return BlockSideEnum.North;
        }

        private static BlockSideEnum MapFromContainerSide(NodeContainerSideEnum nodeContainerIngoingSide)
        {
            if (nodeContainerIngoingSide == NodeContainerSideEnum.Vest)
                return BlockSideEnum.West;
            else if (nodeContainerIngoingSide == NodeContainerSideEnum.North)
                return BlockSideEnum.North;
            else if (nodeContainerIngoingSide == NodeContainerSideEnum.East)
                return BlockSideEnum.East;
            else
                return BlockSideEnum.South;
        }
    }
}
