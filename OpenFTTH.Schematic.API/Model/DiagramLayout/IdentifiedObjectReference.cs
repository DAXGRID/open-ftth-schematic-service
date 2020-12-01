using System;
using System.Collections.Generic;
using System.Text;

namespace OpenFTTH.Schematic.API.Model.DiagramLayout
{
    public class IdentifiedObjectReference
    {
        public Guid RefId { get; set; }
        public String RefClass { get; set; }
    }
}
