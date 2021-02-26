using OpenFTTH.RouteNetwork.API.Model;
using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.Lines;
using OpenFTTH.Util;
using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using System.Collections.Generic;

namespace OpenFTTH.Schematic.Business.NodeSchematic
{
    /// <summary>
    ///  Diagram creation of a span equipment starting, ending or passing through a route network node or element
    /// </summary>
    public class DetachedSpanEquipmentBuilder
    {
        private readonly LookupCollection<SpanStructureSpecification> _spanStructureSpecifications;

        private double _spanEquipmentAreaWidth = 300;
        private double _labelAreaWidth = 100;
        

        public DetachedSpanEquipmentBuilder(LookupCollection<SpanStructureSpecification> spanStructureSpecifications)
        {
            _spanStructureSpecifications = spanStructureSpecifications;
        }

        public IEnumerable<DiagramObject> CreateDiagramObjects(DetachedSpanEquipmentViewModel spanEquipmentViewModel, Diagram diagram, double offsetX, double offsetY)
        {
            return null;
        }

        private LineBlock CreateLabelBlock(List<string> labels)
        {
            var labelBlock = new LineBlock()
            {
                MinWidth = _labelAreaWidth,
            };



            return labelBlock;
        }
    }
}
