using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.Drawing;
using OpenFTTH.Schematic.Business.InternalDiagramObjects.Lines;
using OpenFTTH.Schematic.Business.Layout;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenFTTH.Schematic.Business.Lines
{
    public class LineBlock : DiagramObjectContainer
    {
        public bool IsVisible { get; set; }
        public bool IsSidesVisible { get; set; }

        public VerticalAlignmentEnum VerticalContentAlignment = VerticalAlignmentEnum.Bottom;
        
        private string _style = "LineBlock";
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
           
        public double Margin { get; init; }

        // Reference property
        private Guid _refId = Guid.Empty;

        private string _refClass;

        public void SetReference(Guid refId, string refClass)
        {
            this._refId = refId;
            this._refClass = refClass;
        }

        public int DrawingOrder { get; set; }

        // Desired Size property
        private Size _actualSize = new Size(0, 0);

        public override Size ActualSize => _actualSize;

        // Block sides
        private Dictionary<BlockSideEnum, BlockSide> _sides = new Dictionary<BlockSideEnum, BlockSide>();

        // Connections
        private List<LineBlockPortConnection> _portConnections = new List<LineBlockPortConnection>();

        private List<LineBlockTerminalConnection> _terminalConnections = new List<LineBlockTerminalConnection>();

        public LineBlock(bool isVisible = true)
        {
            this.IsVisible = isVisible;
        }

        public void AddPort(BlockPort port)
        {
            if (!_sides.ContainsKey(port.Side))
                _sides.Add(port.Side, new BlockSide(this, port.Side));

            _sides[port.Side].AddPort(port);
        }

        public LineBlockTerminalConnection AddTerminalConnection(BlockSideEnum fromSide, int fromPortIndex, int fromTerminalIndex, BlockSideEnum toSide, int toPortIndex, int toTerminalIndex, string label = null, string style = null, LineShapeTypeEnum lineShapeType = LineShapeTypeEnum.Line)
        {
            var connection = new LineBlockTerminalConnection();

            connection.Label = label;
            connection.Style = style;
            connection.LineShapeType = lineShapeType;
            connection.FromTerminal = _sides[fromSide].GetPortByIndex(fromPortIndex).GetTerminalByIndex(fromTerminalIndex);

            if (connection.FromTerminal == null)
                throw new Exception("Can't find from terminal side: " + fromSide.ToString() + " port: " + fromPortIndex + " terminal: " + fromTerminalIndex);

            connection.ToTerminal = _sides[toSide].GetPortByIndex(toPortIndex).GetTerminalByIndex(toTerminalIndex);

            if (connection.ToTerminal == null)
                throw new Exception("Can't find to terminal side: " + toSide.ToString() + " port: " + toPortIndex + " terminal: " + toTerminalIndex);


            _terminalConnections.Add(connection);

            return connection;
        }

        public LineBlockPortConnection AddPortConnection(BlockSideEnum fromSide, int fromPortIndex, BlockSideEnum toSide, int toPortIndex, string label = null, string style = null)
        {
            var connection = new LineBlockPortConnection();

            connection.Label = label;
            connection.Style = style;
            connection.FromPort = _sides[fromSide].GetPortByIndex(fromPortIndex);
            connection.ToPort = _sides[toSide].GetPortByIndex(toPortIndex);

            _portConnections.Add(connection);

            return connection;
        }

        private double WidthOfChildren()
        {
            // Calculate width
            double width = 0;

            foreach (var side in _sides.Where(s => s.Key == BlockSideEnum.North || s.Key == BlockSideEnum.South))
            {
                if (side.Value.Length > width)
                    width = side.Value.Length;
            }

            return width;
        }

        private double HeightOfChildren()
        {
            double height = 0;

            foreach (var side in _sides.Where(s => s.Key == BlockSideEnum.West || s.Key == BlockSideEnum.East))
            {
                if (side.Value.Length > height)
                    height = side.Value.Length;
            }

            return height;
        }

        private double HeightOfWestChildren()
        {
            double height = 0;

            foreach (var side in _sides.Where(s => s.Key == BlockSideEnum.West))
            {
                if (side.Value.Length > height)
                    height = side.Value.Length;
            }

            return height;
        }

        private double HeightOfEastChildren()
        {
            double height = 0;

            foreach (var side in _sides.Where(s => s.Key == BlockSideEnum.East))
            {
                if (side.Value.Length > height)
                    height = side.Value.Length;
            }

            return height;
        }


        public override Size Measure()
        {
            // Calculate width
            var width = WidthOfChildren();

            // Add margin
            width += (Margin * 2);

            // If no width, set to 100
            if (width == 0)
                width = 100;

            // Make sure width is at least the min width specified
            if (width < MinWidth)
                width = MinWidth;


            // Calculate height
            var height = HeightOfChildren();

            height += (Margin * 2);

            // If no height, set to 100
            if (height == 0)
                height = 100;

            // Make sure height is at least the min height specified
            if (height < MinHeight)
                height = MinHeight;

            _actualSize = new Size(height, width);

            return _actualSize;
        }

       

        public override IEnumerable<DiagramObject> CreateDiagramObjects(Diagram diagram, double _offsetX, double _offsetY)
        {
            List<DiagramObject> result = new List<DiagramObject>();

            Measure();

            // Create rect to show where block is
            if (IsVisible)
            {
                result.Add(new DiagramObject(diagram)
                {
                    Style = this.Style,
                    Geometry = GeometryBuilder.Rectangle(_offsetX, _offsetY, ActualSize.Height, ActualSize.Width),
                    IdentifiedObject = _refClass == null ? null : new IdentifiedObjectReference() { RefId = _refId, RefClass = _refClass },
                    DrawingOrder = this.DrawingOrder
                });
            }

            // Add all side objects
            foreach (var side in _sides.Values)
            {
                result.AddRange(side.CreateDiagramObjects(diagram, CalculateSideXOffset(side, _offsetX), CalculateSideYOffset(side, _offsetY)));
            }

            // Create all port connections
            foreach (var connection in _portConnections)
            {
                result.AddRange(connection.CreateDiagramObjects(diagram));
            }

            // Create all terminal connections
            foreach (var connection in _terminalConnections)
            {
                result.AddRange(connection.CreateDiagramObjects(diagram));
            }
            
            return result;
        }

        private double CalculateSideXOffset(BlockSide blockSide, double offsetX)
        {
            if (blockSide.Side == BlockSideEnum.West)
                return offsetX;
            else if (blockSide.Side == BlockSideEnum.East)
                return offsetX + ActualSize.Width;
            else if (blockSide.Side == BlockSideEnum.North || blockSide.Side == BlockSideEnum.South)
            {
                if (blockSide.CenterAlignment)
                {
                    double width = WidthOfChildren();
                    double spaceLeft = ActualSize.Width - (width + (Margin * 2));

                    return offsetX + Margin + (spaceLeft / 2);
                }
                else
                {
                    return offsetX + Margin;
                }
            }
            else
                return 0;
        }

        private double CalculateSideYOffset(BlockSide blockSide, double offsetY)
        {
            if (blockSide.Side == BlockSideEnum.West)
            {
                if (VerticalContentAlignment == VerticalAlignmentEnum.Bottom)
                    return offsetY + Margin;
                else
                {
                    double height = HeightOfWestChildren();
                    double spaceLeft = ActualSize.Height - (height + (Margin * 2));
                    return offsetY + spaceLeft + Margin;
                }
            }
            else if (blockSide.Side == BlockSideEnum.East)
            {
                if (VerticalContentAlignment == VerticalAlignmentEnum.Bottom)
                    return offsetY + Margin;
                else
                {
                    double height = HeightOfEastChildren();
                    double spaceLeft = ActualSize.Height - (height + (Margin * 2));
                    return offsetY + spaceLeft + Margin;
                }
            }
            else if (blockSide.Side == BlockSideEnum.North)
                return offsetY + ActualSize.Height;
            else if (blockSide.Side == BlockSideEnum.South)
                return offsetY;
            else
                return 0;
        }

        public void SetSideMargin(BlockSideEnum side, int margin)
        {
            if (_sides.ContainsKey(side))
                _sides[side].SideMargin = margin;
        }

        public void SetSideCenterAlignment(BlockSideEnum side)
        {
            if (_sides.ContainsKey(side))
                _sides[side].CenterAlignment = true;
        }
    }
}
