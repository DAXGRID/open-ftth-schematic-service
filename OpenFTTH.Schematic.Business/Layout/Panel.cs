using System;
using System.Collections.Generic;
using System.Text;

namespace OpenFTTH.Schematic.Business.Layout
{
    public abstract class Panel
    {
        public abstract IEnumerable<DiagramObjectContainer> Children { get;  }
        public abstract void AddChild(DiagramObjectContainer diagramElement);

    }
}
