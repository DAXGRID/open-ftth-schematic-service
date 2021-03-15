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
        private readonly double _typeLabelOffset = 8;

        private Dictionary<Guid, List<TerminalEndHolder>> _terminalEndsByTerminalId = new Dictionary<Guid, List<TerminalEndHolder>>();

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
                Geometry = GeometryBuilder.Point(x, y),
                DrawingOrder = 1000
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
                Geometry = lineGeometry,
                DrawingOrder = 600
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
                Margin = _nodeContainerBlockMargin,
                DrawingOrder = 100
            };

            nodeEquipmentBlock.SetReference(_viewModel.NodeContainer.Id, "NodeContainer");

            AffixConduits(nodeEquipmentBlock);

            ConnectEnds(nodeEquipmentBlock);

            nodeEquipmentBlock.SetSideCenterAlignment(BlockSideEnum.North);
            nodeEquipmentBlock.SetSideCenterAlignment(BlockSideEnum.South);

            return nodeEquipmentBlock;
        }
        private void AffixConduits(LineBlock nodeContainerBlock)
        {
            var attachedSpanEquipments = _viewModel.Data.SpanEquipments.Where(s => s.IsAttachedToNodeContainer(_viewModel.Data)).OrderBy(s => s.SpecificationId);

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

            bool portsVisible = true;

            // If cut we want to draw the port polygons, because otherwise the outer conduit cannot be seen on the diagram (due to missing connection between the two sides)
            if (spanDiagramInfo.IsPassThrough)
                portsVisible = false;

            // Create outer conduit as port
            var fromPort = new BlockPort(fromSide)
            {
                IsVisible = portsVisible,
                Margin = _portMargin,
                Style = spanDiagramInfo.StyleName
            };

            fromPort.DrawingOrder = 420;
            fromPort.SetReference(viewModel.RootSpanDiagramInfo("OuterConduit").IngoingSegmentId, "SpanSegment");
            nodeContainerBlock.AddPort(fromPort);

            var toPort = new BlockPort(toSide)
            {
                IsVisible = portsVisible,
                Margin = _portMargin,
                Style = spanDiagramInfo.StyleName
            };

            toPort.DrawingOrder = 420;
            toPort.SetReference(viewModel.RootSpanDiagramInfo("OuterConduit").OutgoingSegmentId, "SpanSegment");
            
            nodeContainerBlock.AddPort(toPort);

            if (spanDiagramInfo.IsPassThrough)
            {
                var portConnection = nodeContainerBlock.AddPortConnection(fromSide, fromPort.Index, toSide, toPort.Index, null, spanDiagramInfo.StyleName);
                portConnection.SetReference(spanDiagramInfo.SegmentId, "SpanSegment");
                portConnection.DrawingOrder = 410;
            }

            List<SpanDiagramInfo> innerSpanData = null;

            // If a conduit going into west or east side, we want to have inner conduits drawed from top-down along the y-axis
            if (fromSide == BlockSideEnum.West || fromSide == BlockSideEnum.East)
                innerSpanData = viewModel.GetInnerSpanDiagramInfos("InnerConduit").OrderBy(i => (1000 - i.Position)).ToList();
            // Else we just draw them in order allong the x-axis
            else
                innerSpanData = viewModel.GetInnerSpanDiagramInfos("InnerConduit");

            int terminalNo = 1;
            foreach (var data in innerSpanData)
            {
                TerminalShapeTypeEnum terminalShapeType = TerminalShapeTypeEnum.Point;

                // If cut we want to draw the terminal polygon, because otherwise the conduit cannot be seen on the diagram (due to missing connection between the two sides)
                if (!data.IsPassThrough)
                    terminalShapeType = TerminalShapeTypeEnum.PointAndPolygon;

                var fromTerminal = new BlockPortTerminal(fromPort)
                {
                    IsVisible = true,
                    ShapeType = terminalShapeType,
                    PointStyle = fromSide.ToString() + "TerminalLabel",
                    PointLabel = data.IngoingRouteNodeName,
                    PolygonStyle = data.StyleName
                };

                fromTerminal.SetReference(data.IngoingSegmentId, "SpanSegment");
                fromTerminal.DrawingOrder = 520;

                var toTerminal = new BlockPortTerminal(toPort)
                {
                    IsVisible = true,
                    ShapeType = terminalShapeType,
                    PointStyle = toSide.ToString() + "TerminalLabel",
                    PointLabel = data.OutgoingRouteNodeName,
                    PolygonStyle = data.StyleName
                };

                toTerminal.SetReference(data.OutgoingSegmentId, "SpanSegment");
                toTerminal.DrawingOrder = 520;

                // Connect the two sides, if the inner conduit is not cut / passing through
                if (data.IsPassThrough)
                {
                    var terminalConnection = nodeContainerBlock.AddTerminalConnection(fromSide, fromPort.Index, terminalNo, toSide, toPort.Index, terminalNo, null, data.StyleName, LineShapeTypeEnum.Polygon);
                    terminalConnection.SetReference(data.IngoingSegmentId, "SpanSegment");
                    terminalConnection.DrawingOrder = 510;
                }
                else
                {
                    // Add from terminal / ingoing segment to ends
                    if (data.IngoingSpanSegment.FromTerminalId != Guid.Empty)
                        AddToTerminalEnds(data.IngoingSpanSegment.FromTerminalId, data.IngoingSpanSegment, fromTerminal, data.StyleName);

                    if (data.IngoingSpanSegment.ToTerminalId != Guid.Empty)
                        AddToTerminalEnds(data.IngoingSpanSegment.ToTerminalId, data.IngoingSpanSegment, fromTerminal, data.StyleName);

                    // Add to terminal / outgoing segment to ends
                    if (data.OutgoingSpanSegment.FromTerminalId != Guid.Empty)
                        AddToTerminalEnds(data.OutgoingSpanSegment.FromTerminalId, data.OutgoingSpanSegment, toTerminal, data.StyleName);

                    if (data.OutgoingSpanSegment.ToTerminalId != Guid.Empty)
                        AddToTerminalEnds(data.OutgoingSpanSegment.ToTerminalId, data.OutgoingSpanSegment, toTerminal, data.StyleName);
                }

                terminalNo++;
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

            port.DrawingOrder = 420;
            port.SetReference(viewModel.RootSpanDiagramInfo("OuterConduit").SegmentId, "SpanSegment");

            nodeContainerBlock.AddPort(port);

            List<SpanDiagramInfo> innerSpanData = null;

            // If a conduit going into west or east side, we want to have inner conduits drawed from top-down along the y-axis
            if (side == BlockSideEnum.West || side == BlockSideEnum.East)
                innerSpanData = viewModel.GetInnerSpanDiagramInfos("InnerConduit").OrderBy(i => (1000 - i.Position)).ToList();
            // Else we just draw them in order allong the x-axis
            else
                innerSpanData = viewModel.GetInnerSpanDiagramInfos("InnerConduit");


            // Create inner conduits as terminals
            foreach (var data in innerSpanData)
            {
                var terminal = new BlockPortTerminal(port)
                {
                    IsVisible = true,
                    ShapeType = TerminalShapeTypeEnum.PointAndPolygon,
                    PointStyle = side.ToString() + "TerminalLabel",
                    PointLabel = data.OppositeRouteNodeName,
                    PolygonStyle = data.StyleName
                };

                terminal.DrawingOrder = 520;

                terminal.SetReference(data.SegmentId, "SpanSegment");

                if (data.SpanSegment.FromTerminalId != Guid.Empty)
                    AddToTerminalEnds(data.SpanSegment.FromTerminalId, data.SpanSegment, terminal, data.StyleName);

                if (data.SpanSegment.ToTerminalId != Guid.Empty)
                    AddToTerminalEnds(data.SpanSegment.ToTerminalId, data.SpanSegment, terminal, data.StyleName);
            }
        }

        private void ConnectEnds(LineBlock nodeContainerBlock)
        {
            HashSet<BlockPortTerminal> alreadyConnected = new HashSet<BlockPortTerminal>();

            foreach (var terminalEndList in _terminalEndsByTerminalId.Values)
            {
                foreach (var terminalEnd in terminalEndList)
                {
                    if (!alreadyConnected.Contains(terminalEnd.DiagramTerminal))
                    {
                        if (_terminalEndsByTerminalId[terminalEnd.TerminalId].Any(th => th.DiagramTerminal != terminalEnd.DiagramTerminal))
                        {
                            var otherDiagramTerminal = _terminalEndsByTerminalId[terminalEnd.TerminalId].First(th => th.DiagramTerminal != terminalEnd.DiagramTerminal);

                            if (!alreadyConnected.Contains(otherDiagramTerminal.DiagramTerminal))
                            {

                                var terminalConnection = nodeContainerBlock.AddTerminalConnection(
                                    fromSide: terminalEnd.DiagramTerminal.Port.Side, 
                                    fromPortIndex: terminalEnd.DiagramTerminal.Port.Index,
                                    fromTerminalIndex: terminalEnd.DiagramTerminal.Index,
                                    toSide: otherDiagramTerminal.DiagramTerminal.Port.Side,
                                    toPortIndex: otherDiagramTerminal.DiagramTerminal.Port.Index,
                                    toTerminalIndex: otherDiagramTerminal.DiagramTerminal.Index,
                                    label: null,
                                    style: terminalEnd.Style,
                                    lineShapeType: LineShapeTypeEnum.Polygon
                                );

                                terminalConnection.DrawingOrder = 550;

                                // This to make to show branched-out spans on top
                                if (terminalEnd.DiagramTerminal.Port.Side != otherDiagramTerminal.DiagramTerminal.Port.Side)
                                    terminalConnection.DrawingOrder = 560;


                                terminalConnection.SetReference(terminalEnd.SpanSegment.Id, "SpanSegment");

                                alreadyConnected.Add(terminalEnd.DiagramTerminal);
                                alreadyConnected.Add(otherDiagramTerminal.DiagramTerminal);
                            }
                        }
                    }
                }
            }
        }

        private void AddToTerminalEnds(Guid terminalId, SpanSegment spanSegment, BlockPortTerminal digramTerminal, string style)
        {
            var terminalEndHolder = new TerminalEndHolder()
            {
                TerminalId = terminalId,
                SpanSegment = spanSegment,
                DiagramTerminal = digramTerminal,
                Style = style
            };

            if (_terminalEndsByTerminalId.TryGetValue(terminalId, out var terminalEndHolders))
            {
                terminalEndHolders.Add(terminalEndHolder);
            }
            else
            {
                _terminalEndsByTerminalId[terminalId] = new List<TerminalEndHolder>() { terminalEndHolder };
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
            if (nodeContainerIngoingSide == NodeContainerSideEnum.West)
                return BlockSideEnum.West;
            else if (nodeContainerIngoingSide == NodeContainerSideEnum.North)
                return BlockSideEnum.North;
            else if (nodeContainerIngoingSide == NodeContainerSideEnum.East)
                return BlockSideEnum.East;
            else
                return BlockSideEnum.South;
        }

        private class TerminalEndHolder
        {
            public Guid TerminalId { get; set; }
            public SpanSegment  SpanSegment { get; set; }
            public BlockPortTerminal DiagramTerminal { get; set; }
            public string Style { get; set; }
        }
    }
}
