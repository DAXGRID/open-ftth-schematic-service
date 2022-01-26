using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFTTH.Schematic.Business.SchematicBuilder
{
    public class NodeContainerBlockPortViewModel
    {
        public List<NodeContainerBlockTerminalViewModel> TerminalViewModels { get; }

        public NodeContainerBlockPortViewModel(NodeContainerViewModel nodeContainerViewModel, SpanEquipmentViewModel spanEquipmentViewModel, NodeContainerBlockPortViewModelKind kind)
        {

        }
    }
}
