using System;
using System.Collections.Generic;
using System.Text;

namespace OpenFTTH.Schematic.Business.Layout
{
    public class Size
    {
        public Size(double height, double width)
        {
            Height = height;
            Width = width;
        }

        public double Height { get; }
        public double Width { get; }
    }
}
