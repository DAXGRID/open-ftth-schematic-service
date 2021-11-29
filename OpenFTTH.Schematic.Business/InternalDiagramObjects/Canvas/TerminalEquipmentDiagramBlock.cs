using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.Drawing;
using OpenFTTH.Schematic.Business.InternalDiagramObjects.Lines;
using OpenFTTH.Schematic.Business.Layout;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenFTTH.Schematic.Business.Canvas
{
    public class TerminalEquipmentDiagramBlock : DiagramObjectContainer
    {
        public bool IsVisible { get; set; }
        public bool IsSidesVisible { get; set; }

        public VerticalAlignmentEnum VerticalContentAlignment = VerticalAlignmentEnum.Bottom;
        
        private string _style = "CanvasBlock";
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

        public double SpaceBetweenChildren { get; init; }

        // Reference property
        private Guid _refId = Guid.Empty;

        private string _refClass;

        public List<TerminalEquipmentDiagramBlockElement> Children = new();

        public void SetReference(Guid refId, string refClass)
        {
            this._refId = refId;
            this._refClass = refClass;
        }

        public int DrawingOrder { get; set; }

        // Desired Size property
        private Size _actualSize = new Size(0, 0);

        public override Size ActualSize => _actualSize;


        public TerminalEquipmentDiagramBlock(bool isVisible = true)
        {
            this.IsVisible = isVisible;
        }
       

        private double WidthOfChildren()
        {
            // Calculate width
            double width = 0;

            foreach (var child in Children)
            {
                width += child.Measure().Width;
            }

            return width;
        }

        private double HeightOfChildren()
        {
            double height = 0;

            foreach (var child in Children)
            {
                var childHeight = child.Measure().Height;

                if (childHeight > height)
                    height = childHeight;
            }

            return height;
        }

        public override Size Measure()
        {
            // Calculate width
            var width = WidthOfChildren();

            // Add margin
            width += (Margin * 2);

            // Add space between children
            width += (SpaceBetweenChildren * (Children.Count -1));

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

            // Draw canvas, if visible
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

            // Draw children
            var childOffsetX = _offsetX + Margin;
            var childOffsetY = _offsetY + Margin;

            foreach (var child in Children)
            {
                var childSize = child.Measure();

                result.AddRange(child.CreateDiagramObjects(diagram, childOffsetX, childOffsetY));

                childOffsetX += childSize.Width + SpaceBetweenChildren;

            }


            return result;
        }
       
    }
}
