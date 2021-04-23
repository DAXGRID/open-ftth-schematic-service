using OpenFTTH.RouteNetwork.API.Model;
using OpenFTTH.Util;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork.Tracing;
using OpenFTTH.UtilityGraphService.Business.SpanEquipments;
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
        private readonly Guid _elementNodeId;
        private readonly Guid _spanEquipmentId;
        private readonly RouteNetworkElementRelatedData _data;

        private readonly SpanEquipmentWithRelatedInfo _spanEquipment;
        private readonly RouteNetworkInterestRelationKindEnum _relationKind;

        private readonly Dictionary<Guid, RouteNetworkTrace> _traceByBySpanId = new();

        public SpanEquipment SpanEquipment => _spanEquipment;

        public SpanEquipmentViewModel(Guid routeElementId, Guid spanEquipmentId, RouteNetworkElementRelatedData data)
        {
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
            var spanStructure = _spanEquipment.SpanStructures.First(s => s.Level == 1);
            var spec = _data.SpanStructureSpecifications[spanStructure.SpecificationId];

            return GetSpanDiagramInfoForStructure(stylePrefix, spanStructure);
        }

        public string GetSpanEquipmentLabel()
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

            return spanInfos;
        }

        private SpanDiagramInfo GetSpanDiagramInfoForStructure(string stylePrefix, SpanStructure structure)
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
                    spanDiagramInfo.IsPassThrough = true;
                    
                    var spanEquipmentInterest = _data.RouteNetworkInterests[_spanEquipment.WalkOfInterestId];

                    var routeNodeElementIndex = spanEquipmentInterest.RouteNetworkElementRefs.IndexOf(_data.RouteNetworkElementId);
                    var spanSegmentFromIndex = spanEquipmentInterest.RouteNetworkElementRefs.IndexOf(spanSegmentFromRouteNodeId);
                    var spanSegmentToIndex = spanEquipmentInterest.RouteNetworkElementRefs.IndexOf(spanSegmentToRouteNodeId);

                    if (spanSegmentFromIndex < routeNodeElementIndex && spanSegmentToIndex > routeNodeElementIndex)
                    {
                        spanDiagramInfo.IngoingSegmentId = spanSegment.Id;
                        spanDiagramInfo.OutgoingSegmentId = spanSegment.Id;
                        spanDiagramInfo.IngoingSpanSegment = spanSegment;
                        spanDiagramInfo.OutgoingSpanSegment = spanSegment;
                    }
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

        public RouteNetworkInterestRelationKindEnum InterestRelationKind()
        {
            return _data.InterestRelations[_spanEquipment.WalkOfInterestId].RelationKind;
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
