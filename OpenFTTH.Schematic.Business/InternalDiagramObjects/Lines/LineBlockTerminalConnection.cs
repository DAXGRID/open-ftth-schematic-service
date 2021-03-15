using NetTopologySuite.Geometries;
using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.Drawing;
using System;
using System.Collections.Generic;

namespace OpenFTTH.Schematic.Business.Lines
{
    public class LineBlockTerminalConnection
    {
        public LineShapeTypeEnum LineShapeType { get; set; }
        public BlockPortTerminal FromTerminal { get; set; }
        public BlockPortTerminal ToTerminal { get; set; }

        public string Label { get; set; }
        public string Style { get; set; }
        public int DrawingOrder { get; set; }

        private Guid _refId;
        private string _refClass;

        public void SetReference(Guid refId, string refClass)
        {
            this._refId = refId;
            this._refClass = refClass;
        }

        internal IEnumerable<DiagramObject> CreateDiagramObjects(Diagram diagram)
        {
            List<DiagramObject> result = new List<DiagramObject>();

            if (LineShapeType == LineShapeTypeEnum.Polygon)
            {
                NetTopologySuite.Geometries.LineString curve1 = CreateCurve(-4, -4);
                NetTopologySuite.Geometries.LineString curve2 = CreateCurve(+4, +4);

                List<Coordinate> pnts = new List<Coordinate>();

                // Start
                pnts.AddRange(curve1.Coordinates);
                var reversedCurve2 = curve2.Reverse();
                pnts.AddRange(reversedCurve2.Coordinates);
                // complete poly
                pnts.Add(curve1.StartPoint.Coordinate);

                var ring = new LinearRing(pnts.ToArray());

                var poly = new Polygon(ring);

                result.Add(new DiagramObject(diagram)
                {
                    IdentifiedObject = _refClass == null ? null : new IdentifiedObjectReference() { RefId = _refId, RefClass = _refClass },
                    Style = Style is null ? "Cable" : Style,
                    Label = this.Label,
                    Geometry = poly,
                    DrawingOrder = DrawingOrder
                });
            }
            else
            {
                NetTopologySuite.Geometries.LineString curve = CreateCurve();

                result.Add(new DiagramObject(diagram)
                {
                    IdentifiedObject = _refClass == null ? null : new IdentifiedObjectReference() { RefId = _refId, RefClass = _refClass },
                    Style = Style is null ? "Cable" : Style,
                    Label = this.Label,
                    Geometry = curve,
                    DrawingOrder = DrawingOrder
                });
            }

            return result;


        }

        private NetTopologySuite.Geometries.LineString CreateCurve(double shiftX = 0, double shiftY = 0)
        {
            double midX = 0;
            double midY = 0;

            double fromTerminalStartPointX = FromTerminal.ConnectionPointX;
            double fromTerminalStartPointY = FromTerminal.ConnectionPointY;
            double toTerminalEndPointX = ToTerminal.ConnectionPointX;
            double toTerminalEndPointY = ToTerminal.ConnectionPointY;

            double fromCurveStartPointX = 0;
            double fromCurveStartPointY = 0;

            double toCurveEndPointX = 0;
            double toCurveEndPointY = 0;


            // Calculate from curve point
            if (FromTerminal.Port.Side == BlockSideEnum.West)
            {
                fromCurveStartPointX = FromTerminal.ConnectionPointX + FromTerminal.Thickness;
                fromCurveStartPointY = FromTerminal.ConnectionPointY;
            }

            if (FromTerminal.Port.Side == BlockSideEnum.North)
            {
                fromCurveStartPointX = FromTerminal.ConnectionPointX;
                fromCurveStartPointY = FromTerminal.ConnectionPointY - FromTerminal.Thickness;
            }

            if (FromTerminal.Port.Side == BlockSideEnum.East)
            {
                fromCurveStartPointX = FromTerminal.ConnectionPointX - FromTerminal.Thickness;
                fromCurveStartPointY = FromTerminal.ConnectionPointY;
            }

            if (FromTerminal.Port.Side == BlockSideEnum.South)
            {
                fromCurveStartPointX = FromTerminal.ConnectionPointX;
                fromCurveStartPointY = FromTerminal.ConnectionPointY + FromTerminal.Thickness;
            }
                        
            // Calculate to curve point
            if (ToTerminal.Port.Side == BlockSideEnum.West)
            {
                toCurveEndPointX = ToTerminal.ConnectionPointX + FromTerminal.Thickness;
                toCurveEndPointY = ToTerminal.ConnectionPointY;
            }

            if (ToTerminal.Port.Side == BlockSideEnum.North)
            {
                toCurveEndPointX = ToTerminal.ConnectionPointX;
                toCurveEndPointY = ToTerminal.ConnectionPointY - FromTerminal.Thickness;
            }

            if (ToTerminal.Port.Side == BlockSideEnum.East)
            {
                toCurveEndPointX = ToTerminal.ConnectionPointX - FromTerminal.Thickness;
                toCurveEndPointY = ToTerminal.ConnectionPointY;
            }

            if (ToTerminal.Port.Side == BlockSideEnum.South)
            {
                toCurveEndPointX = ToTerminal.ConnectionPointX;
                toCurveEndPointY = ToTerminal.ConnectionPointY + FromTerminal.Thickness;
            }

            // V-N
            if (FromTerminal.Port.Side == BlockSideEnum.West && ToTerminal.Port.Side == BlockSideEnum.North)
            {
                fromCurveStartPointY += shiftY;
                toCurveEndPointX -= shiftX;

                fromTerminalStartPointY += shiftY;
                toTerminalEndPointX -= shiftX;

                midX = toCurveEndPointX;
                midY = fromCurveStartPointY;
            }

            // V-S
            if (FromTerminal.Port.Side == BlockSideEnum.West && ToTerminal.Port.Side == BlockSideEnum.South)
            {
                fromCurveStartPointY += shiftY;
                toCurveEndPointX += shiftX;

                fromTerminalStartPointY += shiftY;
                toTerminalEndPointX += shiftX;

                midX = toCurveEndPointX;
                midY = fromCurveStartPointY;
            }

            // V-E
            if (FromTerminal.Port.Side == BlockSideEnum.West && ToTerminal.Port.Side == BlockSideEnum.East)
            {
                fromCurveStartPointY += shiftY;
                toCurveEndPointY += shiftY;

                fromTerminalStartPointY += shiftY;
                toTerminalEndPointY += shiftY;

                midX = toCurveEndPointX - ((toCurveEndPointX - fromCurveStartPointX) / 2);
                midY = fromCurveStartPointY;
            }

            // N-V
            if (FromTerminal.Port.Side == BlockSideEnum.North && ToTerminal.Port.Side == BlockSideEnum.West)
            {
                fromCurveStartPointX -= shiftX;
                toCurveEndPointY += shiftY;

                fromTerminalStartPointX -= shiftX;
                toTerminalEndPointY += shiftY;

                midX = fromCurveStartPointX;
                midY = toCurveEndPointY;
            }

            // N-E
            if (FromTerminal.Port.Side == BlockSideEnum.North && ToTerminal.Port.Side == BlockSideEnum.East)
            {
                fromCurveStartPointX += shiftX;
                toCurveEndPointY += shiftY;

                fromTerminalStartPointX += shiftX;
                toTerminalEndPointY += shiftY;

                midX = fromCurveStartPointX;
                midY = toCurveEndPointY;
            }

            // N-S
            if (FromTerminal.Port.Side == BlockSideEnum.North && ToTerminal.Port.Side == BlockSideEnum.South)
            {
                fromCurveStartPointX += shiftX;
                toCurveEndPointX += shiftX;

                fromTerminalStartPointX += shiftX;
                toTerminalEndPointX += shiftX;

                midX = fromCurveStartPointX;
                midY = fromCurveStartPointY - ((fromCurveStartPointY - toCurveEndPointY) / 2);
            }

            // E-N
            if (FromTerminal.Port.Side == BlockSideEnum.East && ToTerminal.Port.Side == BlockSideEnum.North)
            {
                fromCurveStartPointY += shiftY;
                toCurveEndPointX += shiftX;

                fromTerminalStartPointY += shiftY;
                toTerminalEndPointX += shiftX;

                midX = toCurveEndPointX;
                midY = fromCurveStartPointY;
            }

            // E-S
            if (FromTerminal.Port.Side == BlockSideEnum.East && ToTerminal.Port.Side == BlockSideEnum.South)
            {
                fromCurveStartPointY += shiftY;
                toCurveEndPointX -= shiftX;

                fromTerminalStartPointY += shiftY;
                toTerminalEndPointX -= shiftX;

                midX = toCurveEndPointX;
                midY = fromCurveStartPointY;
            }

            // E-V
            if (FromTerminal.Port.Side == BlockSideEnum.East && ToTerminal.Port.Side == BlockSideEnum.West)
            {
                fromCurveStartPointY += shiftY;
                toCurveEndPointY += shiftY;

                fromTerminalStartPointY += shiftY;
                toTerminalEndPointY += shiftY;

                midX = fromCurveStartPointX - ((fromCurveStartPointX - toCurveEndPointX) / 2);
                midY = fromCurveStartPointY;
            }

            // S-N
            if (FromTerminal.Port.Side == BlockSideEnum.South && ToTerminal.Port.Side == BlockSideEnum.North)
            {
                fromCurveStartPointX += shiftX;
                toCurveEndPointX += shiftX;

                fromTerminalStartPointX += shiftX;
                toTerminalEndPointX += shiftX;

                midX = fromCurveStartPointX;
                midY = toCurveEndPointY - ((toCurveEndPointY - fromCurveStartPointY) / 2);
            }

            LineString curve = null;

            // straigh line
            if ((fromTerminalStartPointX == toTerminalEndPointX) || (fromTerminalStartPointY == toTerminalEndPointY))
            {
                List<Coordinate> pnts = new List<Coordinate>();
                pnts.Add(new Coordinate(GeometryBuilder.Convert(fromTerminalStartPointX), GeometryBuilder.Convert(fromTerminalStartPointY)));
                pnts.Add(new Coordinate(GeometryBuilder.Convert(toTerminalEndPointX), GeometryBuilder.Convert(toTerminalEndPointY)));
                curve = new LineString(pnts.ToArray());
            }
            else
            {
                curve = GeometryBuilder.Beizer(fromCurveStartPointX, fromCurveStartPointY, midX, midY, toCurveEndPointX, toCurveEndPointY, fromTerminalStartPointX, fromTerminalStartPointY, toTerminalEndPointX, toTerminalEndPointY);
            }
            return curve;
        }
    }
}
