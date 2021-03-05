using NetTopologySuite.Geometries;
using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.Drawing;
using System;
using System.Collections.Generic;

namespace OpenFTTH.Schematic.Business.Lines
{
    public class BlockPortTerminal
    {
        public bool IsVisible { get; init; }

        private string _style = "LinkBlockTerminal";
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

        public TerminalShapeTypeEnum ShapeType { get; init; }

        public string Label { get; init; }

        public BlockPortTerminal(BlockPort port)
        {
            this.Port = port;
            port.AddTerminal(this);
        }

        public BlockPort Port { get; }

        public Point LineConnectionPoint { get; set; }

        public int Index { get; set; }

        public double ConnectionPointX = 0;
        public double ConnectionPointY = 0;

        private double _length = 8;
        public double Length {
            get { return _length; }
            set { _length = value; }
        }

        private Guid _refId;
        private string _refClass;

        public void SetReference(Guid refId, string refClass)
        {
            this._refId = refId;
            this._refClass = refClass;
        }

        public double Thickness
        {
            get { return Port.PortThickness / 2 + (Port.PortThickness / 2); }
        }

        public List<DiagramObject> CreateDiagramObjects(Diagram diagram, double offsetX, double offsetY)
        {

            List<DiagramObject> result = new List<DiagramObject>();

            var terminalOffsetX = offsetX;
            var terminalOffsetY = offsetY;

            ConnectionPointX = 0;
            ConnectionPointY = 0;

            var rectWidth = Port.IsVertical ? Port.PortThickness + (Port.PortThickness / 2) : Length;
            var rectHeight = Port.IsVertical ? Length : Port.PortThickness + (Port.PortThickness / 2);

            if (Port.Side == BlockSideEnum.West)
            {
                //terminalOffsetX += (Port.PortThickness / 2);

                ConnectionPointX = offsetX;
                ConnectionPointY = offsetY + (Length / 2);
            }
            else if (Port.Side == BlockSideEnum.East)
            {
                terminalOffsetX -= (Port.PortThickness + (Port.PortThickness / 2));

                ConnectionPointX = offsetX;
                ConnectionPointY = offsetY + (Length / 2);
            }
            else if (Port.Side == BlockSideEnum.South)
            {
                ConnectionPointX = offsetX + (Length / 2);
                ConnectionPointY = offsetY;
            }
            else if (Port.Side == BlockSideEnum.North)
            {
                terminalOffsetY -= (Port.PortThickness + (Port.PortThickness / 2));  // We need to start on lover y, because we're in the top

                ConnectionPointX = offsetX + (Length / 2);
                ConnectionPointY = offsetY;
            }

            if (IsVisible)
            {
                if (ShapeType == TerminalShapeTypeEnum.Polygon)
                {
                    // Create polygon object convering terminal
                    result.Add(
                        new DiagramObject(diagram)
                        {
                            Style = Style,
                            Label = Label,
                            Geometry = GeometryBuilder.Rectangle(terminalOffsetX, terminalOffsetY, rectHeight, rectWidth),
                            IdentifiedObject = _refClass != null ? new IdentifiedObjectReference() { RefId = _refId, RefClass = _refClass } : null
                        }
                    );
                }
                else if (ShapeType == TerminalShapeTypeEnum.Point)
                {
                    // Create point diagram object at connection point object
                    result.Add(
                        new DiagramObject(diagram)
                        {
                            Style = Style,
                            Label = Label,
                            Geometry = GeometryBuilder.Point(ConnectionPointX, ConnectionPointY),
                            IdentifiedObject = _refClass != null ? new IdentifiedObjectReference() { RefId = _refId, RefClass = _refClass } : null
                        }
                    );
                }
            }

            return result;
        }
    }
}
