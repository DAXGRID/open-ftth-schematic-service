using System;

namespace OpenFTTH.Schematic.Business.SchematicBuilder
{
    public class TerminalEquipmentViewModel
    {
        public Guid TerminalEquipmentId { get; set; }
        public string Name { get; set; }
        public string SpecName { get; set; }
        public int SubrackPosition { get; set; }
        public int SubrackHeight { get; set; }
        public string Style { get; set; }
    }
}
