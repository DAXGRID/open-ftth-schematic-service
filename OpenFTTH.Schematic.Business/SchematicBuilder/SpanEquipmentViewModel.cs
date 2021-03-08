using OpenFTTH.RouteNetwork.API.Model;
using OpenFTTH.Util;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
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

        private readonly SpanEquipment _spanEquipment;
        private readonly RouteNetworkInterestRelationKindEnum _relationKind;

        public SpanEquipment SpanEquipment => _spanEquipment;

        public SpanEquipmentViewModel(Guid routeElementId, Guid spanEquipmentId, RouteNetworkElementRelatedData data)
        {
            _elementNodeId = routeElementId;
            _spanEquipmentId = spanEquipmentId;
            _data = data;

            _spanEquipment = data.SpanEquipments[_spanEquipmentId];
            _relationKind = data.InterestRelations[_spanEquipment.WalkOfInterestId].RelationKind;

            //if (spanEquipment.Traces == null)
            // throw new ApplicationException("SpanEquipment passed to DetachedSpanEquipmentViewModel must contain trace information.");
        }

        public SpanDiagramInfo RootSpanDiagramInfo(string stylePrefix)
        { 
            var spanStructure = _spanEquipment.SpanStructures.First(s => s.Level == 1);
            var spec = _data.SpanStructureSpecifications[spanStructure.SpecificationId];

            return new SpanDiagramInfo() { Position = 1, SpanSegmentId = spanStructure.Id, StyleName = stylePrefix + spec.Color };
        }

        public string GetSpanEquipmentLabel()
        {
            return _data.SpanEquipmentSpecifications[_spanEquipment.SpecificationId].Name;
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
                        var routeNode = _data.RouteNetworkElements[_spanEquipment.NodesOfInterestIds.First()];
                        labels.Add(routeNode.NamingInfo?.Name);
                    }
                    else if (innerLabelDirection == InnerLabelDirectionEnum.Outgoing)
                    {
                        var routeNode = _data.RouteNetworkElements[_spanEquipment.NodesOfInterestIds.Last()];
                        labels.Add(routeNode.NamingInfo?.Name);
                    }
                    else if (innerLabelDirection == InnerLabelDirectionEnum.FromOppositeEndOfNode)
                    {
                        Guid routeNodeId;
                        if (_spanEquipment.NodesOfInterestIds.Last() == _data.RouteNetworkElementId)
                            routeNodeId = _spanEquipment.NodesOfInterestIds.First();
                        else if (_spanEquipment.NodesOfInterestIds.First() == _data.RouteNetworkElementId)
                            routeNodeId = _spanEquipment.NodesOfInterestIds.Last();
                        else
                            throw new ApplicationException("The FromOppositeEndOfNode option can only be used on span equipments that starts or end in the route node. Not pass-through span equipmenst!");

                        var routeNode = _data.RouteNetworkElements[routeNodeId];
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
                var spec = _data.SpanStructureSpecifications[structure.SpecificationId];

                var styleName = stylePrefix + spec.Color;

                styles.Add(new SpanDiagramInfo() { SpanSegmentId = structure.SpanSegments[0].Id, Position = structure.Position, StyleName = styleName });
            }

            return styles;
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
        public string StyleName { get; set; }
        public Guid SpanSegmentId { get; set; }
        public bool IsCut { get; set; }
    }
}
