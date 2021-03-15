using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.Drawing;
using System.Collections.Generic;

namespace OpenFTTH.Schematic.Business.Lines
{
    public class LineBlockConnection
    {
        public BlockPortTerminal FromTerminal { get; set; }
        public BlockPortTerminal ToTerminal { get; set; }

        public string Label { get; set; }
        public string Style { get; set; }
        public int DrawingOrder { get; set; }

        internal IEnumerable<DiagramObject> CreateDiagramObjects(Diagram diagram)
        {
            List<DiagramObject> result = new List<DiagramObject>();

            double midX = 0;
            double midY = 0;

            double fromCurvePointX = 0;
            double fromCurvePointY = 0;

            double toCurvePointX = 0;
            double toCurvePointY = 0;


            // Calculate from curve point
            if (FromTerminal.Port.Side == BlockSideEnum.West)
            {
                fromCurvePointX = FromTerminal.ConnectionPointX + FromTerminal.Thickness;
                fromCurvePointY = FromTerminal.ConnectionPointY;
            }

            if (FromTerminal.Port.Side == BlockSideEnum.North)
            {
                fromCurvePointX = FromTerminal.ConnectionPointX;
                fromCurvePointY = FromTerminal.ConnectionPointY - FromTerminal.Thickness;
            }

            if (FromTerminal.Port.Side == BlockSideEnum.East)
            {
                fromCurvePointX = FromTerminal.ConnectionPointX - FromTerminal.Thickness;
                fromCurvePointY = FromTerminal.ConnectionPointY;
            }

            if (FromTerminal.Port.Side == BlockSideEnum.South)
            {
                fromCurvePointX = FromTerminal.ConnectionPointX;
                fromCurvePointY = FromTerminal.ConnectionPointY + FromTerminal.Thickness;
            }

            // Calculate to curve point
            if (ToTerminal.Port.Side == BlockSideEnum.West)
            {
                toCurvePointX = ToTerminal.ConnectionPointX + FromTerminal.Thickness;
                toCurvePointY = ToTerminal.ConnectionPointY;
            }

            if (ToTerminal.Port.Side == BlockSideEnum.North)
            {
                toCurvePointX = ToTerminal.ConnectionPointX;
                toCurvePointY = ToTerminal.ConnectionPointY - FromTerminal.Thickness;
            }

            if (ToTerminal.Port.Side == BlockSideEnum.East)
            {
                toCurvePointX = ToTerminal.ConnectionPointX - FromTerminal.Thickness;
                toCurvePointY = ToTerminal.ConnectionPointY;
            }

            if (ToTerminal.Port.Side == BlockSideEnum.South)
            {
                toCurvePointX = ToTerminal.ConnectionPointX;
                toCurvePointY = ToTerminal.ConnectionPointY + FromTerminal.Thickness;
            }

            // Calculate mid point
            if ((FromTerminal.Port.Side == BlockSideEnum.West || FromTerminal.Port.Side == BlockSideEnum.East) && (ToTerminal.Port.Side == BlockSideEnum.North || ToTerminal.Port.Side == BlockSideEnum.South))
            {
                midX = ToTerminal.ConnectionPointX;
                midY = FromTerminal.ConnectionPointY;
            }

            if (FromTerminal.Port.Side == BlockSideEnum.West && ToTerminal.Port.Side == BlockSideEnum.East)
            {
                midX = FromTerminal.ConnectionPointX - ((FromTerminal.ConnectionPointY - ToTerminal.ConnectionPointY) / 2);
                midY = FromTerminal.ConnectionPointY;
            }

            if (FromTerminal.Port.Side == BlockSideEnum.North && ToTerminal.Port.Side == BlockSideEnum.South)
            {
                midX = FromTerminal.ConnectionPointX;
                midY = FromTerminal.ConnectionPointY - ((FromTerminal.ConnectionPointY - ToTerminal.ConnectionPointY) / 2);
            }

            if (FromTerminal.Port.Side == BlockSideEnum.South && ToTerminal.Port.Side == BlockSideEnum.East)
            {
                midX = FromTerminal.ConnectionPointX;
                midY = ToTerminal.ConnectionPointY;
            }

            var curve = GeometryBuilder.Beizer(fromCurvePointX, fromCurvePointY, midX, midY, toCurvePointX, toCurvePointY, FromTerminal.ConnectionPointX, FromTerminal.ConnectionPointY, ToTerminal.ConnectionPointX, ToTerminal.ConnectionPointY);

            result.Add(new DiagramObject(diagram)
            {
                Style = Style is null ? "Cable" : Style,
                Label = this.Label,
                Geometry = curve,
                DrawingOrder = DrawingOrder
            }); ; 

            return result;

        }
    }
}
