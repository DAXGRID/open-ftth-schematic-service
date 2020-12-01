using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.Drawing;
using System.Collections.Generic;

namespace OpenFTTH.Schematic.Business.Lines
{
    public class BlockSide
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

        private string _style = "BlockSide";
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

        public BlockSideEnum Side { get; init; }

        public bool CenterAlignment = false;

        private double _sideMargin = 40;

        private double _sideThickness = 10;

        private double _spaceBetweenPorts = 20;

        public double SideMargin
        {
            get { return _sideMargin; }
            set { _sideMargin = value; }
        }

        private List<BlockPort> _ports = new List<BlockPort>();
        private LineBlock _lineBlock = null;


        public BlockSide(LineBlock lineBlock, BlockSideEnum side)
        {
            _lineBlock = lineBlock;
            Side = side;
        }

        public void AddPort(BlockPort port)
        {
            _ports.Add(port);
            port.Index = _ports.Count;
        }

        public BlockPort GetPortByIndex(int index)
        {
            return _ports.Find(p => p.Index == index);
        }

        public double Length
        {
            get
            {
                double length = 0;

                // Sum port lengths
                foreach (var port in _ports)
                    length += port.Length;

                // Add port spaces to length
                if (_ports.Count > 0)
                    length += (_ports.Count - 1) * _spaceBetweenPorts;

                return length + (_sideMargin * 2);
            }
        }

        public List<DiagramObject> CreateDiagramObjects(Diagram diagram, double offsetX, double offsetY)
        {
            List<DiagramObject> result = new List<DiagramObject>();

            if (IsVisible)
            {
                // Create rect diagram object representing the side
                var poly = new DiagramObject(diagram);

                poly.Style = Style;

                var rectWidth = IsVertical ? _sideThickness : Length;
                var rectHeight = IsVertical ? Length : _sideThickness;

                poly.Geometry = GeometryBuilder.Rectangle(offsetX, offsetY, rectHeight, rectWidth);

                result.Add(poly);
            }

            double portX = offsetX;
            double portY = offsetY;

            if (CenterAlignment && (Side == BlockSideEnum.North || Side == BlockSideEnum.South))
            {
                double totalPortLength = 0;

                foreach (var port in _ports)
                    totalPortLength += port.Length;

                double spaceLeft = _lineBlock.MinWidth - totalPortLength;

                double portSpace = spaceLeft / (_ports.Count + 1);
                _sideMargin = portSpace;
                _spaceBetweenPorts = portSpace;
            }

            if (Side == BlockSideEnum.Vest || Side == BlockSideEnum.East)
                portY += _sideMargin;
            else if (Side == BlockSideEnum.North || Side == BlockSideEnum.South)
                portX += _sideMargin;

            foreach (var port in _ports)
            {

                double xStep = 1;
                double yStep = 1;

                if (Side == BlockSideEnum.Vest || Side == BlockSideEnum.East)
                {
                    // goes up y
                    xStep = 0;
                    yStep = port.Length + _spaceBetweenPorts;
                }

                if (Side == BlockSideEnum.North || Side == BlockSideEnum.South)
                {
                    // goes left x
                    xStep = port.Length + _spaceBetweenPorts;
                    yStep = 0;
                }

                result.AddRange(port.CreateDiagramObjects(diagram, portX, portY));

                portX += xStep;
                portY += yStep;
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
