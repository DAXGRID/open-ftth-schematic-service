using OpenFTTH.Schematic.API.Model.DiagramLayout;
using System.Collections.Generic;

namespace OpenFTTH.Schematic.Business.Layout
{
    public abstract class DiagramObjectContainer
    {
        public abstract IEnumerable<DiagramObject> CreateDiagramObjects(Diagram diagram, double offsetX, double offsetY);
        public abstract Size Measure();
        public abstract Size ActualSize { get; }
        public double MinHeight { get; init;  }
        public double MinWidth { get; init; }
        public double ExtraHeightTop { get; init; }
    }
}
