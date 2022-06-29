using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.Canvas;
using OpenFTTH.Schematic.Business.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenFTTH.Schematic.Business.SchematicBuilder
{
    public class TerminalEquipmentBlockBuilder
    {
        NodeContainerViewModel _nodeContainerViewModel;

        public TerminalEquipmentBlockBuilder(NodeContainerViewModel nodeContainerViewModel)
        {
            _nodeContainerViewModel = nodeContainerViewModel;
        }

        public Size Measure()
        {
            var block = CreateEquipmentBlock();

            return block.Measure();
        }

        public TerminalEquipmentDiagramBlock CreateEquipmentBlock(double width = 0)
        {
            var equipmentBlock = new TerminalEquipmentDiagramBlock()
            {
                MinWidth = width,
                IsVisible = false,
                Style = "TerminalEquipmentBlock",
                Margin = 100,
                SpaceBetweenChildren = 100,
                DrawingOrder = 100
            };

            foreach (var rackViewModel in _nodeContainerViewModel.GetRackViewModels().OrderBy(r => r.Name))
            {
                equipmentBlock.Children.Add(new RackDiagramElement(equipmentBlock, rackViewModel));
            }

            foreach (var terminalEquipmentViewModel in _nodeContainerViewModel.GetStandaloneTerminalEquipmentViewModels())
            {
                equipmentBlock.Children.Add(new TerminalEquipmentStandaloneDiagramElement(equipmentBlock, terminalEquipmentViewModel));
            }

            return equipmentBlock;
        }

       
    }
}
