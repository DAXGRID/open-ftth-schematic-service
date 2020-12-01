using NetTopologySuite.Geometries;
using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.Drawing;
using System;
using System.Collections.Generic;

namespace OpenFTTH.Schematic.Business.Lines
{
    public class BlockPortTerminal
    {
        public bool _isVisible = true;
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }

            init
            {
                _isVisible = value;
            }
        }

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


        public string Label { get; init; }

        public BlockPortTerminal(BlockPort port)
        {
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

            // Create terminal diagram object
            var terminalPolygon = new DiagramObject(diagram);

            if (_refClass != null)
                terminalPolygon.IdentifiedObject = new IdentifiedObjectReference() { RefId = _refId, RefClass = _refClass };

            terminalPolygon.Style = Style; 

            var rectWidth = Port.IsVertical ? Port.PortThickness + (Port.PortThickness / 2) : Length;
            var rectHeight = Port.IsVertical ? Length : Port.PortThickness + (Port.PortThickness / 2);

            if (Port.Side == BlockSideEnum.Vest)
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
                terminalOffsetX -= Length;
                terminalOffsetY += (Port.PortThickness / 2);

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
                terminalPolygon.Geometry = GeometryBuilder.Rectangle(terminalOffsetX, terminalOffsetY, rectHeight, rectWidth);

                result.Add(terminalPolygon);

                result.Add(
                    new DiagramObject(diagram)
                    {
                        Style = "LinkBlockTerminalConnectionPoint",
                        Geometry = GeometryBuilder.Point(ConnectionPointX, ConnectionPointY)
                    }
                );
            }

            return result;
        }
    }
}
