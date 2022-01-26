using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFTTH.Schematic.Business.SchematicBuilder
{
   
    public class NodeContainerSpanEquipmentOrganizer
    {
        private List<SpanEquipmentViewModel> _affixedSpanEquipmentViewModels;
        private SpanSegmentEndConnectionMatrix _matrix;
        public SpanSegmentEndConnectionMatrix Matrix => _matrix;

        public NodeContainerSpanEquipmentOrganizer(List<SpanEquipmentViewModel> affixedSpanEquipmentViewModels)
        {
            _affixedSpanEquipmentViewModels = affixedSpanEquipmentViewModels;

            _matrix = CreateConnectionMatrix();
        }

        public List<SpanEquipmentViewModel> SortByConnectivity(List<SpanEquipmentViewModel> spanEquipmentViewModelsToSort)
        {
            Dictionary<Guid, string> sortKey = new();

            foreach (var connectionInfo in _matrix.SpanSegmentEndConnectionInfos.Values)
            {
                if (connectionInfo.SpanEquipmentViewModel.IsSingleSpan)
                {
                    if (connectionInfo.IsConnected)
                    {
                        string sortSuffix = GetSortSuffix(connectionInfo.Side, connectionInfo.ConnectedToSide, connectionInfo.ConnectedToStructureIndex);

                        sortKey[connectionInfo.SpanEquipmentViewModel.SpanEquipment.Id] = $"{connectionInfo.ConnectedToSpanEquipment.Id}-{sortSuffix}";
                    }
                    else
                    {
                        // Put not connected single span last
                        sortKey[connectionInfo.SpanEquipmentViewModel.SpanEquipment.Id] = "x";
                    }
                }
                else
                {
                    // Put everything else last
                    sortKey[connectionInfo.SpanEquipmentViewModel.SpanEquipment.Id] = "z";
                }
            }

            return spanEquipmentViewModelsToSort.OrderBy(s => sortKey[s.SpanEquipment.Id]).ToList();
        }

        private string GetSortSuffix(NodeContainerSideEnum side, NodeContainerSideEnum connectedToSide, ushort connectedToStructureIndex)
        {
            ushort reverseIndexPos = 5000;

            if (side == NodeContainerSideEnum.North && connectedToSide == NodeContainerSideEnum.East)
                return (reverseIndexPos - connectedToStructureIndex).ToString("D7");

            if (side == NodeContainerSideEnum.East && connectedToSide == NodeContainerSideEnum.North)
                return (reverseIndexPos - connectedToStructureIndex).ToString("D7");

            if (side == NodeContainerSideEnum.South && connectedToSide == NodeContainerSideEnum.West)
                return (reverseIndexPos - connectedToStructureIndex).ToString("D7");

            if (side == NodeContainerSideEnum.West && connectedToSide == NodeContainerSideEnum.South)
                return (reverseIndexPos - connectedToStructureIndex).ToString("D7");

            if (side == NodeContainerSideEnum.West && connectedToSide == NodeContainerSideEnum.East)
                return (reverseIndexPos - connectedToStructureIndex).ToString("D7");

            if (side == NodeContainerSideEnum.East && connectedToSide == NodeContainerSideEnum.West)
                return (reverseIndexPos - connectedToStructureIndex).ToString("D7");


            return connectedToStructureIndex.ToString("D7");
        }

        private SpanSegmentEndConnectionMatrix CreateConnectionMatrix()
        {
            SpanSegmentEndConnectionMatrix matrix = new();

            Dictionary<Guid, List<TerminalIndexRecord>> terminalIndex = new();

            // First create an connection info for each ingoing or outgoing span segment in the node container
            foreach (var spanEquipmentView in _affixedSpanEquipmentViewModels)
            {
                foreach (var spanStructure in spanEquipmentView.SpanEquipment.SpanStructures)
                {
                    var spanSegment = GetIngoingOrOutgoingSpanSegment(spanEquipmentView, spanStructure);

                    if (spanSegment != null)
                    {
                        AddToTerminalIndex(terminalIndex, spanSegment.FromTerminalId, spanEquipmentView, spanSegment, spanStructure.Position);
                        AddToTerminalIndex(terminalIndex, spanSegment.ToTerminalId, spanEquipmentView, spanSegment, spanStructure.Position);

                        var connectionInfo = new SpanSegmentEndConnectionInfo()
                        {
                            Side = spanEquipmentView.Affix.NodeContainerIngoingSide,
                            IsConnected = false,
                            SpanEquipmentViewModel = spanEquipmentView,
                            SpanSegment = spanSegment
                        };

                        matrix.SpanSegmentEndConnectionInfos.Add(spanSegment.Id, connectionInfo);
                    }
                }
            }

            // Now connect them together
            foreach (var connectionInfo in matrix.SpanSegmentEndConnectionInfos.Values)
            {
                if (terminalIndex.ContainsKey(connectionInfo.SpanSegment.FromTerminalId))
                {
                    var terminalConnections = terminalIndex[connectionInfo.SpanSegment.FromTerminalId];
                    foreach (var terminalConnection in terminalConnections)
                    {
                        if (terminalConnection.SpanSegment != connectionInfo.SpanSegment)
                        {
                            connectionInfo.ConnectedToSpanEquipment = terminalConnection.SpanEquipmentViewModel.SpanEquipment;
                            connectionInfo.ConnectedToSpanSegment = terminalConnection.SpanSegment;
                            connectionInfo.ConnectedToStructureIndex = terminalConnection.StructurePosition;
                            connectionInfo.ConnectedToSide = terminalConnection.SpanEquipmentViewModel.Affix.NodeContainerIngoingSide;
                            connectionInfo.IsConnected = true;
                        }
                    }
                }
            }

            return matrix;
        }

        private void AddToTerminalIndex(Dictionary<Guid, List<TerminalIndexRecord>> terminalIndex, Guid terminalId, SpanEquipmentViewModel spanEquipmentViewModel, SpanSegment spanSegment, ushort structurePosition)
        {
            if (terminalId == Guid.Empty)
                return;

            if (terminalIndex.ContainsKey(terminalId))
            {
                terminalIndex[terminalId].Add(
                    new TerminalIndexRecord()
                    {
                        SpanEquipmentViewModel = spanEquipmentViewModel,
                        SpanSegment = spanSegment,
                        StructurePosition = structurePosition
                    }
                );
            }
            else
            {
                terminalIndex[terminalId] = new List<TerminalIndexRecord> {
                    new TerminalIndexRecord()
                    {
                        SpanEquipmentViewModel = spanEquipmentViewModel,
                        SpanSegment = spanSegment,
                        StructurePosition = structurePosition
                    }
                };
            }
        }

        private SpanSegment GetIngoingOrOutgoingSpanSegment(SpanEquipmentViewModel spanEquipmentView, SpanStructure spanStructure)
        {
            foreach (var spanSegment in spanStructure.SpanSegments)
            {
                var spanSegmentFromRouteNodeId = spanEquipmentView.SpanEquipment.NodesOfInterestIds[spanSegment.FromNodeOfInterestIndex];
                var spanSegmentToRouteNodeId = spanEquipmentView.SpanEquipment.NodesOfInterestIds[spanSegment.ToNodeOfInterestIndex];

                if (spanSegmentToRouteNodeId == spanEquipmentView.RouteNetworkElementIdOfInterest)
                {
                    return spanSegment;
                }
                else if (spanSegmentFromRouteNodeId == spanEquipmentView.RouteNetworkElementIdOfInterest)
                {
                    return spanSegment;
                }
            }

            return null;
        }
    }

    public class SpanSegmentEndConnectionMatrix
    {
        private Dictionary<Guid, SpanSegmentEndConnectionInfo> _spanSegmentToConnectionInfo = new();

        public Dictionary<Guid, SpanSegmentEndConnectionInfo> SpanSegmentEndConnectionInfos => _spanSegmentToConnectionInfo;
    }

    public class SpanSegmentEndConnectionInfo
    {
        public SpanEquipmentViewModel SpanEquipmentViewModel { get; set; }
        public SpanSegment SpanSegment { get; set; }
        public NodeContainerSideEnum Side { get; set; }
        public bool IsConnected { get; set; }
        public SpanEquipment ConnectedToSpanEquipment { get; set; }
        public SpanSegment ConnectedToSpanSegment { get; set; }
        public ushort ConnectedToStructureIndex { get; set; }
        public NodeContainerSideEnum ConnectedToSide { get; set; }
    }

    public class TerminalIndexRecord
    {
        public SpanEquipmentViewModel SpanEquipmentViewModel { get; set; }
        public SpanSegment SpanSegment { get; set; }
        public ushort StructurePosition { get; set; }
    }
    
}
