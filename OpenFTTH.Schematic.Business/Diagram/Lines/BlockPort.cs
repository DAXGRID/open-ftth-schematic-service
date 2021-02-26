using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.Drawing;
using System;
using System.Collections.Generic;

namespace OpenFTTH.Schematic.Business.Lines
{
    public class BlockPort
    {
        public bool IsVisible { get; set; }

        private double _portMargin = 4;
        private double _portThickness = 10;
        private double _spaceBetweenTerminals = 2;
        private double _terminalSize = -1;
        private string _style = null;
        private string _label = null;

        private List<BlockPortTerminal> _terminals = new List<BlockPortTerminal>();

        public double PortStartX { get; set; }
        public double PortStartY { get; set; }

        public double PortEndX { get; set; }
        public double PortEndY { get; set; }

        private Guid _refId;
        private string _refClass;

        public void SetReference(Guid refId, string refClass)
        {
            this._refId = refId;
            this._refClass = refClass;
        }


        public BlockPort(BlockSideEnum side, string style = null, string label = null, double spaceBetweenTerminals = -1, double terminalSize = -1, double portMargin = -1)
        {
            _side = side;
            _style = style;
            _label = label;

            if (spaceBetweenTerminals > -1)
                _spaceBetweenTerminals = spaceBetweenTerminals;

            if (portMargin > -1)
                _portMargin = portMargin;


            _terminalSize = terminalSize;
        }

        /// <summary>
        /// Notice that if the port is vertical this corresponds to the height, otherwise the width
        /// </summary>
        public double Length
        {
            get
            {
                double space = 0;

                if (_terminals.Count > 0)
                    space = (_terminals.Count - 1) * _spaceBetweenTerminals;

                foreach (var terminal in _terminals)
                    space += terminal.Length;

                return (_portMargin * 2) + space;
            }
        }

        public double PortThickness
        {
            get { return _portThickness; }
        }

        // Side
        private readonly BlockSideEnum _side;
        public BlockSideEnum Side => _side;

        public int Index { get; set; }

        public IEnumerable<BlockPortTerminal> Terminals
        {
            get { return _terminals; }
        }

        public void AddTerminal(BlockPortTerminal terminal)
        {
            _terminals.Add(terminal);
            terminal.Index = _terminals.Count;

            if (_terminalSize != -1)
                terminal.Length = _terminalSize;
        }

        public BlockPortTerminal GetTerminalByIndex(int index)
        {
            return _terminals.Find(t => t.Index == index);
        }

        public List<DiagramObject> CreateDiagramObjects(Diagram diagram, double offsetX, double offsetY)
        {
            List<DiagramObject> result = new List<DiagramObject>();

            var portOffsetX = offsetX;
            var portOffsetY = offsetY;

            var rectWidth = IsVertical ? _portThickness : Length;
            var rectHeight = IsVertical ? Length : _portThickness;

            PortStartX = offsetX;
            PortStartY = offsetY;
            PortEndX = IsVertical ? PortStartX : PortStartX + Length;
            PortEndY = IsVertical ? PortStartY + Length : PortStartY;

            if (_side == BlockSideEnum.North)
            {
                portOffsetY -= _portThickness; // We need to start on lover y, because we're in the top
            }
            else if (_side == BlockSideEnum.East)
            {
                portOffsetX -= _portThickness;
            }

            if (IsVisible)
            {
                // Create port diagram object
                var portPolygon = new DiagramObject(diagram);

                if (_refClass != null)
                    portPolygon.IdentifiedObject = new IdentifiedObjectReference() { RefId = _refId, RefClass = _refClass };

                portPolygon.Style = _style ?? "BlockPort";

                portPolygon.Geometry = GeometryBuilder.Rectangle(portOffsetX, portOffsetY, rectHeight, rectWidth);

                result.Add(portPolygon);
            }

            // Create terminal diagram objects
            double terminalX = offsetX;
            double terminalY = offsetY;

            if (_side == BlockSideEnum.Vest || _side == BlockSideEnum.East)
                terminalY += _portMargin;
            else if (_side == BlockSideEnum.North || _side == BlockSideEnum.South)
                terminalX += _portMargin;

            foreach (var terminal in _terminals)
            {

                double xStep = 1;
                double yStep = 1;

                if (_side == BlockSideEnum.Vest || _side == BlockSideEnum.East)
                {
                    // goes up y
                    xStep = 0;
                    yStep = terminal.Length + _spaceBetweenTerminals;
                }

                if (_side == BlockSideEnum.North)
                {
                    // goes right along x
                    xStep = terminal.Length + _spaceBetweenTerminals;
                    yStep = 0;
                }

                if (_side == BlockSideEnum.South)
                {
                    // goes right along x
                    xStep = terminal.Length + _spaceBetweenTerminals;
                    yStep = 0;
                }


                result.AddRange(terminal.CreateDiagramObjects(diagram, terminalX, terminalY));

                terminalX += xStep;
                terminalY += yStep;
            }

            return result;
        }

        public bool IsVertical
        {
            get
            {
                if (Side == BlockSideEnum.East || Side == BlockSideEnum.Vest)
                    return true;
                else
                    return false;
            }
        }
    }
}
