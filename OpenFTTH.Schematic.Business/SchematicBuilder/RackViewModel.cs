using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFTTH.Schematic.Business.SchematicBuilder
{
    public class RackViewModel
    {
        public Guid RackId { get; set; }
        public string Name { get; set; }
        public int MinHeightInUnits { get; set; }
        public string SpecName { get; set; }
        public List<TerminalEquipmentViewModel> TerminalEquipments { get; set; }    
    }
}
