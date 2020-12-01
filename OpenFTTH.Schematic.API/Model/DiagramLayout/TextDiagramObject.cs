using System;
using System.Collections.Generic;
using System.Text;

namespace OpenFTTH.Schematic.API.Model.DiagramLayout
{
    public class TextDiagramObject : DiagramObject
    {
        public string Text { get; set; }

        public TextDiagramObject(Diagram diagram) : base(diagram)
        {

        }
    }
}
