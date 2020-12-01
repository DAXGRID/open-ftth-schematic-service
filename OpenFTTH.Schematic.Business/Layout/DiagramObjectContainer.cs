using OpenFTTH.Schematic.API.Model.DiagramLayout;
using System.Collections.Generic;

namespace OpenFTTH.Schematic.Business.Layout
{
    public abstract class DiagramObjectContainer
    {
        public abstract IEnumerable<DiagramObject> CreateDiagramObjects(Diagram diagram, double offsetX, double offsetY);
        public abstract Size Measure(Size availableSize);
        public abstract Size Arrange(Size finalSize);
        public abstract Size DesiredSize { get; }
        public double MinHeight { get; set;  }
        public double MinWidth { get; set; }
        public double ActualHeight { get; }
        public double ActualWidth { get; }
    }
}
