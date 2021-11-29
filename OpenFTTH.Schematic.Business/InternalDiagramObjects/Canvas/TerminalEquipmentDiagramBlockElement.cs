using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.Layout;
using System.Collections.Generic;

namespace OpenFTTH.Schematic.Business.Canvas
{
    public abstract class TerminalEquipmentDiagramBlockElement
    {
        public abstract IEnumerable<DiagramObject> CreateDiagramObjects(Diagram diagram, double offsetX, double offsetY);

        public abstract Size Measure();
    }
}
