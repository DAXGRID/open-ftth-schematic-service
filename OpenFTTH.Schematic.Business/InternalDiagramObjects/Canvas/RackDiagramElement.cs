using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.Drawing;
using OpenFTTH.Schematic.Business.Layout;
using OpenFTTH.Schematic.Business.SchematicBuilder;
using System;
using System.Collections.Generic;


namespace OpenFTTH.Schematic.Business.Canvas
{
    public class RackDiagramElement : TerminalEquipmentDiagramBlockElement
    {
        RackViewModel _rackViewModel;

        static double _rackWidth = 200;
        static double _rackUnitHeight = 5;
        static double _innerFrameMargin = 40;

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

        public RackDiagramElement(TerminalEquipmentDiagramBlock canvasBlock, RackViewModel rackViewModel)
        {
            _rackViewModel = rackViewModel;
            _terminalEquipmentBlock = canvasBlock;
            Width = _rackWidth;
            Height = (_rackUnitHeight * rackViewModel.MinHeightInUnits) + (_innerFrameMargin * 2);
        }
   
        public override List<DiagramObject> CreateDiagramObjects(Diagram diagram, double offsetX, double offsetY)
        {
            List<DiagramObject> result = new List<DiagramObject>();

            // Create rack block rectangle
            var rackPoly = new DiagramObject(diagram)
            {
                Geometry = GeometryBuilder.Rectangle(offsetX, offsetY, Height, Width),
                Style = "Rack",
                DrawingOrder = _terminalEquipmentBlock.DrawingOrder + (ushort)100,
                IdentifiedObject = new IdentifiedObjectReference() { RefClass = "Rack", RefId = _rackViewModel.RackId }
            };

            result.Add(rackPoly);

            // Create rack block rectangle
            var subRackSpacePoly = new DiagramObject(diagram)
            {
                Geometry = GeometryBuilder.Rectangle(offsetX + _innerFrameMargin, offsetY + _innerFrameMargin, Height - (_innerFrameMargin * 2), Width - (_innerFrameMargin * 2)),
                Style = "SubrackSpace",
                DrawingOrder = _terminalEquipmentBlock.DrawingOrder + (ushort)200
            };

            result.Add(subRackSpacePoly);


            // Create Unit texts
            for (int rackUnit = 0; rackUnit <= _rackViewModel.MinHeightInUnits; rackUnit++)
            {
                result.Add(
                    new DiagramObject(diagram)
                    {
                        Style = "RackUnitLabel",
                        Label = $"U {rackUnit}",
                        Geometry = GeometryBuilder.Point(offsetX + 40, offsetY + (rackUnit * _rackUnitHeight) + 40),
                        DrawingOrder = _terminalEquipmentBlock.DrawingOrder + (ushort)300
                    }
                );
            }

            // Create rack label
            result.Add(CreateRackLabel(diagram, offsetX + _innerFrameMargin, offsetY + 15 + Height - _innerFrameMargin, _rackViewModel.Name));


            // Create terminal equipments in rack
            foreach (var rackMount in _rackViewModel.TerminalEquipments)
            {
                result.AddRange(CreateTerminalEquipment(diagram, offsetX + _innerFrameMargin, offsetY + _innerFrameMargin + (rackMount.SubrackPosition * _rackUnitHeight), rackMount.SubrackHeight * _rackUnitHeight, rackMount));
            }

            return result;
        }

        private List<DiagramObject> CreateTerminalEquipment(Diagram diagram, double offsetX, double offsetY, double height, TerminalEquipmentViewModel rackMount)
        {
            List<DiagramObject> result = new();

            var terminalEquipmentBlockWidth = Width - (_innerFrameMargin * 2);

            // Create terminal equipment rectangle
            result.Add(
                new DiagramObject(diagram)
                {
                    Geometry = GeometryBuilder.Rectangle(offsetX, offsetY, height, terminalEquipmentBlockWidth),
                    Style = rackMount.Style,
                    DrawingOrder = _terminalEquipmentBlock.DrawingOrder + (ushort)400,
                    IdentifiedObject = new IdentifiedObjectReference() { RefClass = "TerminalEquipment", RefId = rackMount.TerminalEquipmentId }
                }
            );

            if (height >= 40)
            {
                double distanceBetwenNameAndType = 20;

                double nameTextOffset = (height / 2) + (distanceBetwenNameAndType / 2);
                double typeTextOffset = (height / 2) - (distanceBetwenNameAndType / 2);

                result.Add(CreateTerminalEquipmentNameLabel(diagram, offsetX + (terminalEquipmentBlockWidth / 2), offsetY + nameTextOffset, rackMount.Name));
                result.Add(CreateTerminalEquipmentTypeLabel(diagram, offsetX + (terminalEquipmentBlockWidth / 2), offsetY + typeTextOffset, "(" + rackMount.SpecName + ")"));
            }
            else
            {
                double textOffset = (height / 2);

                result.Add(CreateTerminalEquipmentNameLabel(diagram, offsetX + (terminalEquipmentBlockWidth / 2), offsetY + textOffset, rackMount.Name + " (" + rackMount.SpecName + ")"));
            }

            return result;
        }



        private DiagramObject CreateRackLabel(Diagram diagram, double x, double y, string label)
        {
            var labelDiagramObject = new DiagramObject(diagram)
            {
                Style = "RackLabel",
                Label = label,
                Geometry = GeometryBuilder.Point(x, y),
                DrawingOrder = _terminalEquipmentBlock.DrawingOrder + (ushort)400
            };

            return labelDiagramObject;
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
