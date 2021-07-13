﻿using NetTopologySuite.Geometries;
using OpenFTTH.RouteNetwork.API.Model;
using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.Drawing;
using OpenFTTH.Schematic.Business.InternalDiagramObjects.Lines;
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
        private readonly NodeContainerViewModel _nodeContainerViewModel;

        private readonly double _areaWidth = 300;
        private readonly double _nodeContainerBlockMargin = 60;
        private readonly double _portMargin = 10;
        private readonly double _typeLabelOffset = 8;

        private Dictionary<Guid, List<TerminalEndHolder>> _terminalEndsByTerminalId = new Dictionary<Guid, List<TerminalEndHolder>>();

        public NodeContainerBuilder(NodeContainerViewModel viewModel)
        {
            _nodeContainerViewModel = viewModel;
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
                Label = _nodeContainerViewModel.GetNodeContainerTypeLabel(),
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
                DrawingOrder = 700
            };

            containerSide.SetReference(_nodeContainerViewModel.NodeContainer.Id, "NodeContainer");

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
                DrawingOrder = 100,
                VerticalContentAlignment = GetContainerVerticalAlignment()
            };

            nodeEquipmentBlock.SetReference(_nodeContainerViewModel.NodeContainer.Id, "NodeContainer");

            AffixConduits(nodeEquipmentBlock);

            ConnectEnds(nodeEquipmentBlock);

            nodeEquipmentBlock.SetSideCenterAlignment(BlockSideEnum.North);
            nodeEquipmentBlock.SetSideCenterAlignment(BlockSideEnum.South);

            return nodeEquipmentBlock;
        }

        private VerticalAlignmentEnum GetContainerVerticalAlignment()
        {
            if (_nodeContainerViewModel.NodeContainer.VertialContentAlignmemt == NodeContainerVerticalContentAlignmentEnum.Top)
                return VerticalAlignmentEnum.Top;
            else
                return VerticalAlignmentEnum.Bottom;
        }


        private void AffixConduits(LineBlock nodeContainerBlock)
        {
            var spanEquipmentViewModels = new List<SpanEquipmentViewModel>();
                
            foreach (var spanEquipment in _nodeContainerViewModel.Data.SpanEquipments.Where(s => s.IsAttachedToNodeContainer(_nodeContainerViewModel.Data)))
                spanEquipmentViewModels.Add(new SpanEquipmentViewModel(_nodeContainerViewModel.Data.RouteNetworkElementId, spanEquipment.Id, _nodeContainerViewModel.Data));

            foreach (var viewModel in GetOrderedSpanEquipmentViewModels(spanEquipmentViewModels))
            {
                AffixConduit(nodeContainerBlock, viewModel);
            }
        }

        private List<SpanEquipmentViewModel> GetOrderedSpanEquipmentViewModels(List<SpanEquipmentViewModel> spanEquipmentViewModels)
        {
            List<SpanEquipmentViewModel> toBeDrawedFirstList = new List<SpanEquipmentViewModel>();
            List<SpanEquipmentViewModel> toBeDrawedSecondList = new List<SpanEquipmentViewModel>();

            // Make sure that span equipments that is pass through is drawed first
            foreach (var spanEquipmentViewModel in spanEquipmentViewModels)
            {
                if (spanEquipmentViewModel.IsPassThrough)
                    toBeDrawedFirstList.Add(spanEquipmentViewModel);
                else
                    toBeDrawedSecondList.Add(spanEquipmentViewModel);
            }

            // Sort by marking color
            toBeDrawedFirstList = toBeDrawedFirstList.OrderBy(s => (GetOrderByKey(s.SpanEquipment))).ToList();

            if (_nodeContainerViewModel.NodeContainer.VertialContentAlignmemt == NodeContainerVerticalContentAlignmentEnum.Bottom)
            {
                toBeDrawedFirstList.AddRange(toBeDrawedSecondList);
                return toBeDrawedFirstList;
            }
            else
            {
                toBeDrawedSecondList.AddRange(toBeDrawedFirstList);
                return toBeDrawedSecondList;
            }
        }

        private bool SidesAreOppsite(NodeContainerSideEnum nodeContainerIngoingSide1, NodeContainerSideEnum nodeContainerIngoingSide2)
        {
            if (nodeContainerIngoingSide1 == NodeContainerSideEnum.West && nodeContainerIngoingSide2 == NodeContainerSideEnum.East)
                return true;

            if (nodeContainerIngoingSide1 == NodeContainerSideEnum.North && nodeContainerIngoingSide2 == NodeContainerSideEnum.South)
                return true;

            if (nodeContainerIngoingSide1 == NodeContainerSideEnum.East && nodeContainerIngoingSide2 == NodeContainerSideEnum.West)
                return true;

            if (nodeContainerIngoingSide1 == NodeContainerSideEnum.South && nodeContainerIngoingSide2 == NodeContainerSideEnum.North)
                return true;

            return false;
        }

        private string GetOrderByKey(SpanEquipment spanSegment)
        {
            return spanSegment.SpecificationId.ToString() + (spanSegment.MarkingInfo?.MarkingColor);
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
                Style = spanDiagramInfo.StyleName,
                PointStyle = fromSide.ToString() + "TerminalLabel",
                PointLabel = viewModel.GetSpanEquipmentLabel()
            };

            fromPort.DrawingOrder = 420;
            fromPort.SetReference(viewModel.RootSpanDiagramInfo("OuterConduit").IngoingSegmentId, "SpanSegment");
            nodeContainerBlock.AddPort(fromPort);

            var toPort = new BlockPort(toSide)
            {
                IsVisible = portsVisible,
                Margin = _portMargin,
                Style = spanDiagramInfo.StyleName,
                PointStyle = toSide.ToString() + "TerminalLabel",
                PointLabel = viewModel.GetSpanEquipmentLabel()
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

            bool innerSpansFound = false;

            int terminalNo = 1;

            foreach (var innerSpan in innerSpanData)
            {
                TerminalShapeTypeEnum terminalShapeType = TerminalShapeTypeEnum.Point;

                // If cut we want to draw the terminal polygon, because otherwise the conduit cannot be seen on the diagram (due to missing connection between the two sides)
                if (!innerSpan.IsPassThrough)
                    terminalShapeType = TerminalShapeTypeEnum.PointAndPolygon;


                string fromNodeName = "NA";
                string toNodeName = "NA";

                if (innerSpan.IsPassThrough)
                {
                    fromNodeName = viewModel.GetIngoingRouteNodeName(innerSpan.SegmentId);
                    toNodeName = viewModel.GetOutgoingRouteNodeName(innerSpan.SegmentId);
                }
                else
                {
                    //if (viewModel.InterestRelationKind() == RouteNetworkInterestRelationKindEnum.Start)
                    
                    fromNodeName = viewModel.GetIngoingRouteNodeName(innerSpan.IngoingSpanSegment.Id);
                    toNodeName = viewModel.GetOutgoingRouteNodeName(innerSpan.OutgoingSpanSegment.Id);
                }

                var fromTerminal = new BlockPortTerminal(fromPort)
                {
                    IsVisible = true,
                    ShapeType = terminalShapeType,
                    PointStyle = fromSide.ToString() + "TerminalLabel",
                    PointLabel = fromNodeName,
                    PolygonStyle = innerSpan.StyleName
                };

                fromTerminal.SetReference(innerSpan.IngoingSegmentId, "SpanSegment");
                fromTerminal.DrawingOrder = 620;

                var toTerminal = new BlockPortTerminal(toPort)
                {
                    IsVisible = true,
                    ShapeType = terminalShapeType,
                    PointStyle = toSide.ToString() + "TerminalLabel",
                    PointLabel = toNodeName,
                    PolygonStyle = innerSpan.StyleName
                };

                toTerminal.SetReference(innerSpan.OutgoingSegmentId, "SpanSegment");
                toTerminal.DrawingOrder = 620;

                // Connect the two sides, if the inner conduit is not cut / passing through
                if (innerSpan.IsPassThrough)
                {
                    var terminalConnection = nodeContainerBlock.AddTerminalConnection(fromSide, fromPort.Index, terminalNo, toSide, toPort.Index, terminalNo, null, innerSpan.StyleName, LineShapeTypeEnum.Polygon);
                    terminalConnection.SetReference(innerSpan.IngoingSegmentId, "SpanSegment");
                    terminalConnection.DrawingOrder = 510;
                }
                else
                {
                    // Add from terminal / ingoing segment to ends
                    if (innerSpan.IngoingSpanSegment != null && innerSpan.IngoingSpanSegment.FromTerminalId != Guid.Empty)
                        AddToTerminalEnds(innerSpan.IngoingSpanSegment.FromTerminalId, innerSpan.IngoingSpanSegment, fromTerminal, innerSpan.StyleName);

                    if (innerSpan.IngoingSpanSegment != null && innerSpan.IngoingSpanSegment.ToTerminalId != Guid.Empty)
                        AddToTerminalEnds(innerSpan.IngoingSpanSegment.ToTerminalId, innerSpan.IngoingSpanSegment, fromTerminal, innerSpan.StyleName);

                    // Add to terminal / outgoing segment to ends
                    if (innerSpan.OutgoingSpanSegment != null && innerSpan.OutgoingSpanSegment.FromTerminalId != Guid.Empty)
                        AddToTerminalEnds(innerSpan.OutgoingSpanSegment.FromTerminalId, innerSpan.OutgoingSpanSegment, toTerminal, innerSpan.StyleName);

                    if (innerSpan.OutgoingSpanSegment != null && innerSpan.OutgoingSpanSegment.ToTerminalId != Guid.Empty)
                        AddToTerminalEnds(innerSpan.OutgoingSpanSegment.ToTerminalId, innerSpan.OutgoingSpanSegment, toTerminal, innerSpan.StyleName);
                }

                terminalNo++;
                innerSpansFound = true;
            }

            // Create fake inner terminals used to display where the empty multi conduit is heading
            if (!innerSpansFound)
            {
                var fromTerminal = new BlockPortTerminal(fromPort)
                {
                    IsVisible = true,
                    ShapeType = TerminalShapeTypeEnum.Point,
                    PointStyle = fromSide.ToString() + "TerminalLabel",
                    PointLabel = viewModel.GetIngoingRouteNodeName(spanDiagramInfo.IngoingSegmentId),
                    DrawingOrder = 520
                };

                fromTerminal.SetReference(spanDiagramInfo.IngoingSegmentId, "SpanSegment");

                var toTerminal = new BlockPortTerminal(toPort)
                {
                    IsVisible = true,
                    ShapeType = TerminalShapeTypeEnum.Point,
                    PointStyle = toSide.ToString() + "TerminalLabel",
                    PointLabel = viewModel.GetOutgoingRouteNodeName(spanDiagramInfo.IngoingSegmentId),
                    DrawingOrder = 520
                };

                toTerminal.SetReference(spanDiagramInfo.OutgoingSegmentId, "SpanSegment");

                terminalNo++;
            }

        }

        private void AffixConduitEnd(LineBlock nodeContainerBlock, SpanEquipmentViewModel viewModel)
        {
            var spanDiagramInfo = viewModel.RootSpanDiagramInfo("OuterConduit");

            BlockSideEnum side = MapFromContainerSide(viewModel.Affix.NodeContainerIngoingSide);

            // Port
            var port = new BlockPort(side)
            {
                IsVisible = viewModel.IsSingleSpan ? false: true,
                Margin = viewModel.IsSingleSpan ? _portMargin / 2 : _portMargin,
                Style = viewModel.RootSpanDiagramInfo("OuterConduit").StyleName,
                PointStyle = side.ToString() + "TerminalLabel",
                PointLabel = viewModel.GetSpanEquipmentLabel()
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

            bool innerSpansFound = false;

            // Create inner conduits as terminals
            foreach (var data in innerSpanData)
            {
                var terminal = new BlockPortTerminal(port)
                {
                    IsVisible = true,
                    ShapeType = TerminalShapeTypeEnum.PointAndPolygon,
                    PointStyle = side.ToString() + "TerminalLabel",
                    PointLabel = viewModel.InterestRelationKind() == RouteNetworkInterestRelationKindEnum.End ? viewModel.GetIngoingRouteNodeName(data.SegmentId) : viewModel.GetOutgoingRouteNodeName(data.SegmentId),
                    PolygonStyle = data.StyleName
                };

                terminal.DrawingOrder = 620;

                terminal.SetReference(data.SegmentId, "SpanSegment");

                if (data.SpanSegment != null && data.SpanSegment.FromTerminalId != Guid.Empty)
                    AddToTerminalEnds(data.SpanSegment.FromTerminalId, data.SpanSegment, terminal, data.StyleName);

                if (data.SpanSegment != null && data.SpanSegment.ToTerminalId != Guid.Empty)
                    AddToTerminalEnds(data.SpanSegment.ToTerminalId, data.SpanSegment, terminal, data.StyleName);

                innerSpansFound = true;
            }

            // Create fake inner terminal used to display where the empty multi conduit is heading
            if (!innerSpansFound)
            {
                var terminal = new BlockPortTerminal(port)
                {
                    IsVisible = true,
                    ShapeType = viewModel.IsSingleSpan ? TerminalShapeTypeEnum.PointAndPolygon : TerminalShapeTypeEnum.Point,
                    PointStyle = side.ToString() + "TerminalLabel",
                    PointLabel = viewModel.InterestRelationKind() == RouteNetworkInterestRelationKindEnum.End ? viewModel.GetIngoingRouteNodeName(spanDiagramInfo.SegmentId) : viewModel.GetOutgoingRouteNodeName(spanDiagramInfo.SegmentId),
                    PolygonStyle = spanDiagramInfo.StyleName,
                    DrawingOrder = 620
                };

                terminal.SetReference(spanDiagramInfo.SegmentId, "SpanSegment");

                if (viewModel.IsSingleSpan)
                {
                    if (spanDiagramInfo.SpanSegment != null && spanDiagramInfo.SpanSegment.FromTerminalId != Guid.Empty)
                        AddToTerminalEnds(spanDiagramInfo.SpanSegment.FromTerminalId, spanDiagramInfo.SpanSegment, terminal, spanDiagramInfo.StyleName);

                    if (spanDiagramInfo.SpanSegment != null && spanDiagramInfo.SpanSegment.ToTerminalId != Guid.Empty)
                        AddToTerminalEnds(spanDiagramInfo.SpanSegment.ToTerminalId, spanDiagramInfo.SpanSegment, terminal, spanDiagramInfo.StyleName);
                }
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
            public SpanSegment SpanSegment { get; set; }
            public BlockPortTerminal DiagramTerminal { get; set; }
            public string Style { get; set; }
        }
    }
}
