using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.Drawing;
using OpenFTTH.Schematic.Business.Layout;
using OpenFTTH.Schematic.Business.SchematicBuilder;
using System.Collections.Generic;


namespace OpenFTTH.Schematic.Business.Canvas
{
    public class TerminalEquipmentStandaloneDiagramElement : TerminalEquipmentDiagramBlockElement
    {
        static double _width = 200;
        static double _height = 200;

        TerminalEquipmentViewModel _terminalEquipmentViewModel;

        private string _style = "Rack";
        public string Style
        {
            get
            {
                return _style;
            }

            init
            {
                _style = value;
            }
        }

        private TerminalEquipmentDiagramBlock _terminalEquipmentBlock = null;

        private double Width { get; }
        private double Height { get; }

        public TerminalEquipmentStandaloneDiagramElement(TerminalEquipmentDiagramBlock canvasBlock, TerminalEquipmentViewModel terminalEquipmentViewModel)
        {
            _terminalEquipmentViewModel = terminalEquipmentViewModel;
            _terminalEquipmentBlock = canvasBlock;
            Width = _width;
            Height = _height;
        }
   
        public override List<DiagramObject> CreateDiagramObjects(Diagram diagram, double offsetX, double offsetY)
        {
            List<DiagramObject> result = new List<DiagramObject>();

            // Create terminal equipment rectangle
            var terminalEquipmentPoly = new DiagramObject(diagram)
            {
                Geometry = GeometryBuilder.Rectangle(offsetX, offsetY, Height, Width),
                Style = _terminalEquipmentViewModel.Style,
                DrawingOrder = _terminalEquipmentBlock.DrawingOrder + (ushort)400,
                IdentifiedObject = new IdentifiedObjectReference() { RefClass = "TerminalEquipment", RefId = _terminalEquipmentViewModel.TerminalEquipmentId }
            };

            result.Add(terminalEquipmentPoly);

            // Create name label
            result.Add(CreateTerminalEquipmentNameLabel(diagram, offsetX + (Width / 2), offsetY + (Width / 2) + 30, _terminalEquipmentViewModel.Name));
            result.Add(CreateTerminalEquipmentTypeLabel(diagram, offsetX + (Width / 2), offsetY + (Width / 2) + 70, "(" + _terminalEquipmentViewModel.SpecName + ")"));

            return result;
        }

        private DiagramObject CreateTerminalEquipmentNameLabel(Diagram diagram, double x, double y, string label)
        {
            var labelDiagramObject = new DiagramObject(diagram)
            {
                Style = "TerminalEquipmentNameLabel",
                Label = label,
                Geometry = GeometryBuilder.Point(x, y),
                DrawingOrder = _terminalEquipmentBlock.DrawingOrder + (ushort)500
            };

            return labelDiagramObject;
        }

        private DiagramObject CreateTerminalEquipmentTypeLabel(Diagram diagram, double x, double y, string label)
        {
            var labelDiagramObject = new DiagramObject(diagram)
            {
                Style = "TerminalEquipmentTypeLabel",
                Label = label,
                Geometry = GeometryBuilder.Point(x, y),
                DrawingOrder = _terminalEquipmentBlock.DrawingOrder + (ushort)500
            };

            return labelDiagramObject;
        }

        public override Size Measure()
        {
            return new Size(Height, Width);
        }
    }
}
