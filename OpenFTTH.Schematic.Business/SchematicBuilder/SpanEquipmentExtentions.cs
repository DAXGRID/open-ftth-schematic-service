using OpenFTTH.RouteNetwork.API.Model;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using System.Linq;

namespace OpenFTTH.Schematic.Business.SchematicBuilder
{
    public static class SpanEquipmentExtensions
    {
        public static bool IsAttachedToNodeContainer(this SpanEquipment spanEquipment, RouteNetworkElementRelatedData data)
        {
            if (data.NodeContainer == null)
                return false;

            if (spanEquipment.NodeContainerAffixes != null)
            {
                foreach (var affix in spanEquipment.NodeContainerAffixes)
                {
                    if (affix.NodeContainerId == data.NodeContainer.Id)
                        return true;
                }
            }

            return false;
        }

        public static bool IsPassThrough(this SpanEquipment spanEquipment, RouteNetworkElementRelatedData data)
        {
            if (spanEquipment.NodesOfInterestIds.First() != data.RouteNetworkElementId && spanEquipment.NodesOfInterestIds.Last() != data.RouteNetworkElementId)
                return true;

            return false;
        }

    }
}
