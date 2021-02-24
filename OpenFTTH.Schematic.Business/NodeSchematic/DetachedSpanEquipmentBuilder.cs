using OpenFTTH.RouteNetwork.API.Model;
using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.Lines;
using OpenFTTH.Util;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using System.Collections.Generic;

namespace OpenFTTH.Schematic.Business.NodeSchematic
{
    public class DetachedSpanEquipmentBuilder
    {
        private readonly LookupCollection<SpanStructureSpecification> _spanStructureSpecifications;

        private double _labelBlockWidth = 100;

        public DetachedSpanEquipmentBuilder(LookupCollection<SpanStructureSpecification> spanStructureSpecifications)
        {
            _spanStructureSpecifications = spanStructureSpecifications;
        }

        public IEnumerable<DiagramObject> CreateDiagramObjects(SpanEquipment spanEquipment, Diagram diagram, double offsetX, double offsetY)
        {

            return null;
        }

        private LineBlock CreateLabelBlock(List<RouteNetworkElement> routeNetworkElements)
        {
            var labelBlock = new LineBlock()
            {
                MinWidth = _labelBlockWidth,
            };


            return labelBlock;
        }

    }
}
