using OpenFTTH.RouteNetwork.API.Model;
using OpenFTTH.Util;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenFTTH.Schematic.Business.SchematicBuilder
{
    /// <summary>
    /// View model serving diagram creation of a span equipment starting, ending or passing through a route network node or element
    /// </summary>
    public class DetachedSpanEquipmentViewModel
    {
        private readonly SpanEquipmentWithRelatedInfo _spanEquipment;
        private readonly Guid _elementNodeId;
        private readonly LookupCollection<RouteNetworkElement> _routeNetworkElements;
        private readonly LookupCollection<SpanStructureSpecification> _spanStructureSpecifications;

        public DetachedSpanEquipmentViewModel(Guid routeElementId, SpanEquipmentWithRelatedInfo spanEquipment, LookupCollection<RouteNetworkElement> routeNetworkElements, LookupCollection<SpanStructureSpecification> spanStructureSpecifications)
        {
            _spanEquipment = spanEquipment;
            _elementNodeId = routeElementId;
            _routeNetworkElements = routeNetworkElements;
            _spanStructureSpecifications = spanStructureSpecifications;

            //if (spanEquipment.Traces == null)
            // throw new ApplicationException("SpanEquipment passed to DetachedSpanEquipmentViewModel must contain trace information.");
        }

        public SpanDiagramInfo RootSpanDiagramInfo(string stylePrefix)
        { 
            var spanStructure = _spanEquipment.SpanStructures.First(s => s.Level == 1);
            var spec = _spanStructureSpecifications[spanStructure.SpecificationId];

            return new SpanDiagramInfo() { Position = 1, SpanSegmentId = spanStructure.Id, StyleName = stylePrefix + spec.Color };
        }

        public List<string> GetInnerSpanLabels(InnerLabelDirectionEnum innerLabelDirection)
        {
            var innerStructures = _spanEquipment.SpanStructures.Where(s => s.Level == 2);

            List<string> labels = new List<string>();

            foreach (var structure in innerStructures)
            {
                if (!structure.ContainsCutOrConnectedSpanSegments())
                {
                    if (innerLabelDirection == InnerLabelDirectionEnum.Ingoing)
                    {
                        var routeNode = _routeNetworkElements[_spanEquipment.WalkOfInterest.RouteNetworkElementRefs.First()];
                        labels.Add(routeNode.NamingInfo?.Name);
                    }
                    else
                    {
                        var routeNode = _routeNetworkElements[_spanEquipment.WalkOfInterest.RouteNetworkElementRefs.Last()];
                        labels.Add(routeNode.NamingInfo?.Name);
                    }
                }
            }

            return labels;
        }

        public List<SpanDiagramInfo> GetInnerSpanDiagramInfos(string stylePrefix)
        {
            var innerStructures = _spanEquipment.SpanStructures.Where(s => s.Level == 2);

            List<SpanDiagramInfo> styles = new List<SpanDiagramInfo>();

            foreach (var structure in innerStructures)
            {
                var spec = _spanStructureSpecifications[structure.SpecificationId];

                var styleName = stylePrefix + spec.Color;

                styles.Add(new SpanDiagramInfo() { SpanSegmentId = structure.Id, Position = structure.Position, StyleName = styleName });
            }

            return styles;
        }
    }

    public class SpanDiagramInfo
    {
        public int Position { get; set; }
        public string StyleName { get; set; }
        public Guid SpanSegmentId { get; set; }
    }
}
