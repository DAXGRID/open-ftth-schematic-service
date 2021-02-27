using OpenFTTH.UtilityGraphService.API.Model.UtilityNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFTTH.Schematic.Business.SchematicBuilder
{
    public static class SpanStructureExtensions
    {
        public static bool ContainsCutOrConnectedSpanSegments(this SpanStructure spanStructure)
        {
            if (spanStructure.SpanSegments == null && spanStructure.SpanSegments.Length == 0)
                return false;

            if (spanStructure.SpanSegments.Length == 1 && spanStructure.SpanSegments[0].FromTerminalId == Guid.Empty && spanStructure.SpanSegments[0].ToTerminalId == Guid.Empty)
                return false;

            return true;
        }
    }
}
