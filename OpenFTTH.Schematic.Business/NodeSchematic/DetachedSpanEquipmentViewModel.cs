using OpenFTTH.RouteNetwork.API.Model;
using OpenFTTH.Util;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenFTTH.Schematic.Business.NodeSchematic
{
    /// <summary>
    /// View model serving diagram creation of a span equipment starting, ending or passing through a route network node or element
    /// </summary>
    public class DetachedSpanEquipmentViewModel
    {
        private readonly SpanEquipmentWithRelatedInfo _spanEquipment;
        private readonly Guid _elementNodeId;
        private readonly LookupCollection<RouteNetworkElement> _routeNetworkElements;

        public DetachedSpanEquipmentViewModel(Guid routeElementId, SpanEquipmentWithRelatedInfo spanEquipment, LookupCollection<RouteNetworkElement> routeNetworkElements)
        {
            _spanEquipment = spanEquipment;
            _elementNodeId = routeElementId;
            _routeNetworkElements = routeNetworkElements;

            //if (spanEquipment.Traces == null)
            // throw new ApplicationException("SpanEquipment passed to DetachedSpanEquipmentViewModel must contain trace information.");
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
    }
}
