using Microsoft.Extensions.Logging;
using OpenFTTH.RouteNetwork.API.Model;
using OpenFTTH.Schematic.Business.Lines;
using OpenFTTH.Schematic.Business.QueryHandler;
using OpenFTTH.UtilityGraphService.API.Model.Trace;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenFTTH.Schematic.Business.SchematicBuilder
{
    /// <summary>
    /// View model serving diagram creation of a span equipment starting, ending or passing through a route network node or element
    /// </summary>
    public class SpanEquipmentViewModel
    {
        private readonly ILogger<GetDiagramQueryHandler> _logger;
        private readonly Guid _elementNodeId;
        private readonly Guid _spanEquipmentId;
        private readonly RouteNetworkElementRelatedData _data;

        public RouteNetworkElementRelatedData Data => _data;

        private readonly SpanEquipmentWithRelatedInfo _spanEquipment;
        private readonly RouteNetworkInterestRelationKindEnum _relationKind;

        private readonly Dictionary<Guid, RouteNetworkTraceResult> _traceByBySpanId = new();

        public SpanEquipment SpanEquipment => _spanEquipment;

        public Guid RouteNetworkElementIdOfInterest => _data.RouteNetworkElementId;

        private SpanDiagramInfo _cachedComputedRouteDiagramInfo = null;

        public SpanEquipmentViewModel(ILogger<GetDiagramQueryHandler> logger, Guid routeElementId, Guid spanEquipmentId, RouteNetworkElementRelatedData data)
        {
            _logger = logger;
            _elementNodeId = routeElementId;
            _spanEquipmentId = spanEquipmentId;
            _data = data;

            _spanEquipment = data.SpanEquipments[_spanEquipmentId];
            _relationKind = data.InterestRelations[_spanEquipment.WalkOfInterestId].RelationKind;

            if (_spanEquipment.RouteNetworkTraceRefs == null)
                throw new ApplicationException("SpanEquipment passed to DetachedSpanEquipmentViewModel must contain trace information.");

            foreach (var traceRef in _spanEquipment.RouteNetworkTraceRefs)
            {
                var trace = _data.RouteNetworkTraces[traceRef.TraceId];
                _traceByBySpanId.Add(traceRef.SpanEquipmentOrSegmentId, trace);
            }
        }

        public SpanDiagramInfo RootSpanDiagramInfo(string stylePrefix)
        {
            if (_cachedComputedRouteDiagramInfo != null)
                return _cachedComputedRouteDiagramInfo;

            var spanStructure = _spanEquipment.SpanStructures.First(s => s.Level == 1);
            var spec = _data.SpanStructureSpecifications[spanStructure.SpecificationId];

            _cachedComputedRouteDiagramInfo = GetSpanDiagramInfoForStructure(stylePrefix, spanStructure);

            return _cachedComputedRouteDiagramInfo;
        }

        public string GetConduitEquipmentLabel()
        {
            string label = _data.SpanEquipmentSpecifications[_spanEquipment.SpecificationId].Name;

            if (_spanEquipment.MarkingInfo != null && _spanEquipment.MarkingInfo.MarkingColor != null)
            {
                label += " " + _spanEquipment.MarkingInfo.MarkingColor;
            }

            if (_spanEquipment.MarkingInfo != null && _spanEquipment.MarkingInfo.MarkingText != null)
            {
                label += " " + _spanEquipment.MarkingInfo.MarkingText;
            }

            return label;
        }

       

        public List<SpanDiagramInfo> GetInnerSpanDiagramInfos(string stylePrefix)
        {
            var innerStructures = _spanEquipment.SpanStructures.Where(s => s.Level == 2 && s.Deleted == false);

            List<SpanDiagramInfo> spanInfos = new List<SpanDiagramInfo>();

            foreach (var structure in innerStructures)
            {
                spanInfos.Add(GetSpanDiagramInfoForStructure(stylePrefix, structure));
            }

            // If a conduit going into west or east side, we want to have inner conduits drawed from top-down along the y-axis
            if (BlockSideWhereSpanEquipmentShouldBeAffixed() == BlockSideEnum.West || BlockSideWhereSpanEquipmentShouldBeAffixed() == BlockSideEnum.East)
                return spanInfos.OrderBy(i => (1000 - i.Position)).ToList();
            // Else we just draw them in order along the x-axis reflected by their structure position
            else
                return spanInfos;
        }

        public SpanDiagramInfo GetSpanDiagramInfoForStructure(string stylePrefix, SpanStructure structure)
        {
            var spec = _data.SpanStructureSpecifications[structure.SpecificationId];

            var spanDiagramInfo = new SpanDiagramInfo()
            {
                StyleName = stylePrefix + spec.Color,
                Position = structure.Position
            };

            foreach (var spanSegment in structure.SpanSegments)
            {
                var spanSegmentFromRouteNodeId = _spanEquipment.NodesOfInterestIds[spanSegment.FromNodeOfInterestIndex];
                var spanSegmentToRouteNodeId = _spanEquipment.NodesOfInterestIds[spanSegment.ToNodeOfInterestIndex];

                if (spanSegmentToRouteNodeId == _data.RouteNetworkElementId)
                {
                    spanDiagramInfo.IsIngoing = true;
                    spanDiagramInfo.IsPassThrough = false;
                    spanDiagramInfo.IngoingSegmentId = spanSegment.Id;
                    spanDiagramInfo.IngoingSpanSegment = spanSegment;
                }
                else if (spanSegmentFromRouteNodeId == _data.RouteNetworkElementId)
                {
                    spanDiagramInfo.IsIngoing = false;
                    spanDiagramInfo.IsPassThrough = false;
                    spanDiagramInfo.OutgoingSegmentId = spanSegment.Id;
                    spanDiagramInfo.OutgoingSpanSegment = spanSegment;
                }
                else
                {
                    var spanEquipmentInterest = _data.RouteNetworkInterests[_spanEquipment.WalkOfInterestId];

                    var routeNodeElementIndex = spanEquipmentInterest.RouteNetworkElementRefs.IndexOf(_data.RouteNetworkElementId);

                    var spanSegmentFromIndex = spanEquipmentInterest.RouteNetworkElementRefs.IndexOf(spanSegmentFromRouteNodeId);
                    var spanSegmentToIndex = spanEquipmentInterest.RouteNetworkElementRefs.IndexOf(spanSegmentToRouteNodeId);

                    // Check if segment is passing the route node we are looking at (build a diagram for)
                    if (spanSegmentFromIndex < routeNodeElementIndex && spanSegmentToIndex > routeNodeElementIndex)
                    {
                        spanDiagramInfo.IsPassThrough = true;

                        spanDiagramInfo.IngoingSegmentId = spanSegment.Id;
                        spanDiagramInfo.OutgoingSegmentId = spanSegment.Id;
                        spanDiagramInfo.IngoingSpanSegment = spanSegment;
                        spanDiagramInfo.OutgoingSpanSegment = spanSegment;
                    }
                }
            }

            // Log detailed error information if no segments are parsing by or ending in the route node element we're looking at
            if (spanDiagramInfo.IngoingSegmentId == Guid.Empty && spanDiagramInfo.OutgoingSegmentId == Guid.Empty)
            {
                _logger.LogError($"Error relating any span segments in structure level: {structure.Level} position: {structure.Position} in span equipment with id: {this._spanEquipment.Id} to route node with id: {_data.RouteNetworkElementId}");

                foreach (var spanSegment in structure.SpanSegments)
                {
                    var spanSegmentFromRouteNodeId = _spanEquipment.NodesOfInterestIds[spanSegment.FromNodeOfInterestIndex];
                    var spanSegmentToRouteNodeId = _spanEquipment.NodesOfInterestIds[spanSegment.ToNodeOfInterestIndex];

                    _logger.LogError($"Potential wrong from or to node id in span segment: {spanSegment.Id} FromRouteNodeId: {spanSegmentFromRouteNodeId} ToRouteNodeId: {spanSegmentToRouteNodeId}");
                }
            }


            return spanDiagramInfo;
        }

        public string GetIngoingRouteNodeName(Guid spanSegmentId)
        {
            if (_traceByBySpanId.TryGetValue(spanSegmentId, out var routeNetworkTraceBySegment))
            {
                return routeNetworkTraceBySegment.FromRouteNodeName;
            }
            else if (_traceByBySpanId.TryGetValue(_spanEquipment.Id, out var routeNetworkTraceByEquipment))
            {
                return routeNetworkTraceByEquipment.FromRouteNodeName;
            }
            else
                throw new ApplicationException("Can't find incoming route node name in route network traces.");
                
            //return _data.RouteNetworkElements[_spanEquipment.NodesOfInterestIds.First()].Name;
        }

        public string GetOutgoingRouteNodeName(Guid spanSegmentId)
        {
            if (_traceByBySpanId.TryGetValue(spanSegmentId, out var routeNetworkTraceBySegment))
            {
                return routeNetworkTraceBySegment.ToRouteNodeName;
            }
            else if (_traceByBySpanId.TryGetValue(_spanEquipment.Id, out var routeNetworkTraceByEquipment))
            {
                return routeNetworkTraceByEquipment.ToRouteNodeName;
            }
            else
                throw new ApplicationException("Can't find incoming route node name in route network traces.");
        }

        public bool IsAttachedToNodeContainer()
        {
            if (_data.NodeContainer == null)
                return false;

            if (_spanEquipment.NodeContainerAffixes != null)
            {
                foreach (var affix in _spanEquipment.NodeContainerAffixes)
                {
                    if (affix.NodeContainerId == _data.NodeContainer.Id)
                        return true;
                }
            }

            return false;
        }

        public bool IsPassThrough
        {
            get
            {
                if (_spanEquipment.NodesOfInterestIds.First() != _data.RouteNetworkElementId && _spanEquipment.NodesOfInterestIds.Last() != _data.RouteNetworkElementId)
                    return true;

                return false;
            }
        }

        public SpanEquipmentNodeContainerAffix Affix
        {
            get
            {
                if (_spanEquipment.NodeContainerAffixes != null)
                {
                    foreach (var affix in _spanEquipment.NodeContainerAffixes)
                    {
                        if (affix.NodeContainerId == _data.NodeContainer.Id)
                            return affix;
                    }
                }

                throw new ApplicationException($"Span equipment: {_spanEquipment.Id} not affixed to node container at route node: {_data.RouteNetworkElementId}. Please use IsAttachedToNodeContainer to avoid this exception.");
            }
        }

        public bool IsSingleSpan { 
            get
            {
                var spec = _data.SpanEquipmentSpecifications[_spanEquipment.SpecificationId];

                return (!spec.IsMultiLevel);
            }
        }

        public RouteNetworkInterestRelationKindEnum InterestRelationKind()
        {
            return _data.InterestRelations[_spanEquipment.WalkOfInterestId].RelationKind;
        }

        public bool IsCableWithinConduit
        {
            get
            {
                if (_data.CableToConduitSegmentParentRelations.ContainsKey(this.SpanEquipment.Id))
                    return true;

                return false;
            }
        }

        public BlockSideEnum BlockSideWhereSpanEquipmentShouldBeAffixed()
        {
            if (!IsAttachedToNodeContainer())
                return BlockSideEnum.West;

            if (Affix.NodeContainerIngoingSide == NodeContainerSideEnum.West)
                return BlockSideEnum.West;
            else if (Affix.NodeContainerIngoingSide == NodeContainerSideEnum.North)
                return BlockSideEnum.North;
            else if (Affix.NodeContainerIngoingSide == NodeContainerSideEnum.East)
                return BlockSideEnum.East;
            else
                return BlockSideEnum.South;
        }

        public BlockSideEnum OppositeBlockSideWhereSpanEquipmentShouldBeAffixed()
        {
            if (!IsAttachedToNodeContainer())
                return BlockSideEnum.East;

            if (Affix.NodeContainerIngoingSide == NodeContainerSideEnum.West)
                return BlockSideEnum.East;
            else if (Affix.NodeContainerIngoingSide == NodeContainerSideEnum.North)
                return BlockSideEnum.South;
            else if (Affix.NodeContainerIngoingSide == NodeContainerSideEnum.East)
                return BlockSideEnum.West;
            else
                return BlockSideEnum.North;
        }

    }

    public class SpanDiagramInfo
    {
        public int Position { get; set; }
        public bool IsPassThrough { get; set; }
        public bool IsIngoing { get; set; }
        public string StyleName { get; set; }
        public Guid IngoingSegmentId { get; set; }
        public Guid OutgoingSegmentId { get; set; }
        public SpanSegment IngoingSpanSegment { get; set; }
        public SpanSegment OutgoingSpanSegment { get; set; }
        public SpanSegment SpanSegment
        {
            get
            {
                if (IngoingSegmentId != Guid.Empty)
                {
                    return IngoingSpanSegment;
                }
                else
                {
                    return OutgoingSpanSegment;
                }
            }
        }

        public Guid SegmentId
        {
            get
            {
                if (IngoingSegmentId != Guid.Empty)
                {
                    return IngoingSegmentId;
                }
                else
                {
                    return OutgoingSegmentId;
                }
            }
        }

        
    }
}
