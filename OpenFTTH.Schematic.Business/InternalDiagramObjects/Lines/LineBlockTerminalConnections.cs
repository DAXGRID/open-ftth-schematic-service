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
        private Envelope _canvasEnvelope = null;


        public void SetReference(Guid refId, string refClass)
        {
            this._refId = refId;
            this._refClass = refClass;
        }

        internal IEnumerable<DiagramObject> CreateDiagramObjects(Diagram diagram)
        {
            _canvasEnvelope = diagram.Envelope;

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
                NetTopologySuite.Geometries.LineString curve = CreateCurve(0,0,0);

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

        private NetTopologySuite.Geometries.LineString CreateCurve(double shiftX = 0, double shiftY = 0, double extraLengthBothEnds = 0)
        {
            double midX1 = 0;
            double midY1 = 0;

            double midX2 = -1;
            double midY2 = -1;

            bool sameSide = false;


            double fromTerminalStartPointX = FromTerminal.ConnectionPointX;
            double fromTerminalPointY = FromTerminal.ConnectionPointY;
            double toTerminalPointX = ToTerminal.ConnectionPointX;
            double toTerminalPointY = ToTerminal.ConnectionPointY;

            double fromCurvePointX = 0;
            double fromCurvePointY = 0;

            double toCurvePointX = 0;
            double toCurvePointY = 0;


            // Calculate from curve point
            if (FromTerminal.Port.Side == BlockSideEnum.West)
            {
                fromCurvePointX = FromTerminal.ConnectionPointX + FromTerminal.Thickness;
                fromCurvePointY = FromTerminal.ConnectionPointY;

                fromTerminalStartPointX -= extraLengthBothEnds;
            }

            if (FromTerminal.Port.Side == BlockSideEnum.North)
            {
                fromCurvePointX = FromTerminal.ConnectionPointX;
                fromCurvePointY = FromTerminal.ConnectionPointY - FromTerminal.Thickness;

                fromTerminalPointY += extraLengthBothEnds;
            }

            if (FromTerminal.Port.Side == BlockSideEnum.East)
            {
                fromCurvePointX = FromTerminal.ConnectionPointX - FromTerminal.Thickness;
                fromCurvePointY = FromTerminal.ConnectionPointY;

                fromTerminalStartPointX += extraLengthBothEnds;
            }

            if (FromTerminal.Port.Side == BlockSideEnum.South)
            {
                fromCurvePointX = FromTerminal.ConnectionPointX;
                fromCurvePointY = FromTerminal.ConnectionPointY + FromTerminal.Thickness;

                fromTerminalPointY += extraLengthBothEnds;
            }
                        
            // Calculate to curve point
            if (ToTerminal.Port.Side == BlockSideEnum.West)
            {
                toCurvePointX = ToTerminal.ConnectionPointX + FromTerminal.Thickness;
                toCurvePointY = ToTerminal.ConnectionPointY;

                toTerminalPointX -= extraLengthBothEnds;
            }

            if (ToTerminal.Port.Side == BlockSideEnum.North)
            {
                toCurvePointX = ToTerminal.ConnectionPointX;
                toCurvePointY = ToTerminal.ConnectionPointY - FromTerminal.Thickness;

                toTerminalPointY += extraLengthBothEnds;
            }

            if (ToTerminal.Port.Side == BlockSideEnum.East)
            {
                toCurvePointX = ToTerminal.ConnectionPointX - FromTerminal.Thickness;
                toCurvePointY = ToTerminal.ConnectionPointY + extraLengthBothEnds;

                toTerminalPointX += extraLengthBothEnds;
            }

            if (ToTerminal.Port.Side == BlockSideEnum.South)
            {
                toCurvePointX = ToTerminal.ConnectionPointX;
                toCurvePointY = ToTerminal.ConnectionPointY + FromTerminal.Thickness;

                toTerminalPointY -= extraLengthBothEnds;
            }

            // V-N
            if (FromTerminal.Port.Side == BlockSideEnum.West && ToTerminal.Port.Side == BlockSideEnum.North)
            {
                fromCurvePointY += shiftY;
                toCurvePointX -= shiftX;

                fromTerminalPointY += shiftY;
                toTerminalPointX -= shiftX;

                midX1 = toCurvePointX;
                midY1 = fromCurvePointY;
            }

            // N-V
            if (FromTerminal.Port.Side == BlockSideEnum.North && ToTerminal.Port.Side == BlockSideEnum.West)
            {
                fromCurvePointX -= shiftX;
                toCurvePointY += shiftY;

                fromTerminalStartPointX -= shiftX;
                toTerminalPointY += shiftY;

                midX1 = fromCurvePointX;
                midY1 = toCurvePointY;
            }

            // V-S
            if (FromTerminal.Port.Side == BlockSideEnum.West && ToTerminal.Port.Side == BlockSideEnum.South)
            {
                fromCurvePointY += shiftY;
                toCurvePointX += shiftX;

                fromTerminalPointY += shiftY;
                toTerminalPointX += shiftX;

                midX1 = toCurvePointX;
                midY1 = fromCurvePointY;
            }

            // S-V
            if (FromTerminal.Port.Side == BlockSideEnum.South && ToTerminal.Port.Side == BlockSideEnum.West)
            {
                //TODO
                fromCurvePointY += shiftY;
                toCurvePointX += shiftX;

                fromTerminalPointY += shiftY;
                toTerminalPointX += shiftX;

                midX1 = fromCurvePointX;
                midY1 = toCurvePointY;
            }


            // N-E
            if (FromTerminal.Port.Side == BlockSideEnum.North && ToTerminal.Port.Side == BlockSideEnum.East)
            {
                fromCurvePointX += shiftX;
                toCurvePointY += shiftY;

                fromTerminalStartPointX += shiftX;
                toTerminalPointY += shiftY;

                midX1 = fromCurvePointX;
                midY1 = toCurvePointY;
            }

            // E-N
            if (FromTerminal.Port.Side == BlockSideEnum.East && ToTerminal.Port.Side == BlockSideEnum.North)
            {
                fromCurvePointY += shiftY;
                toCurvePointX += shiftX;

                fromTerminalPointY += shiftY;
                toTerminalPointX += shiftX;

                midX1 = toCurvePointX;
                midY1 = fromCurvePointY;
            }

            // N-S
            if (FromTerminal.Port.Side == BlockSideEnum.North && ToTerminal.Port.Side == BlockSideEnum.South)
            {
                fromCurvePointX += shiftX;
                toCurvePointX += shiftX;

                fromTerminalStartPointX += shiftX;
                toTerminalPointX += shiftX;

                var xSpan = fromCurvePointX > toCurvePointX ? fromCurvePointX - toCurvePointX : toCurvePointX - fromCurvePointX;

                var ySpan = fromCurvePointY > toCurvePointY ? fromCurvePointY - toCurvePointY : toCurvePointY - fromCurvePointY;

                midY2 = toCurvePointY + (ySpan / 3);
                midY1 = toCurvePointY + ((ySpan / 3) * 2);

                if (fromCurvePointX > toCurvePointX)
                {
                    midX2 = toCurvePointX + (0.1 * xSpan);
                    midX1 = toCurvePointX + (0.9 * xSpan);
                }
                else
                {
                    midX2 = fromCurvePointX + (0.9 * xSpan);
                    midX1 = fromCurvePointX + (0.1 * xSpan);
                }
            }

            // S-N
            if (FromTerminal.Port.Side == BlockSideEnum.South && ToTerminal.Port.Side == BlockSideEnum.North)
            {
                fromCurvePointX += shiftX;
                toCurvePointX += shiftX;

                fromTerminalStartPointX += shiftX;
                toTerminalPointX += shiftX;

                var xSpan = fromCurvePointX > toCurvePointX ? fromCurvePointX - toCurvePointX : toCurvePointX - fromCurvePointX;

                var ySpan = fromCurvePointY > toCurvePointY ? fromCurvePointY - toCurvePointY : toCurvePointY - fromCurvePointY;

                midY1 = fromCurvePointY + (ySpan / 3);
                midY2 = fromCurvePointY + ((ySpan / 3) * 2);

                if (fromCurvePointX > toCurvePointX)
                {
                    midX1 = toCurvePointX + (0.9 * xSpan);
                    midX2 = toCurvePointX + (0.1 * xSpan);
                }
                else
                {
                    midX1 = fromCurvePointX + (0.1 * xSpan);
                    midX2 = fromCurvePointX + (0.9 * xSpan);
                }
            }

            // E-S
            if (FromTerminal.Port.Side == BlockSideEnum.East && ToTerminal.Port.Side == BlockSideEnum.South)
            {
                fromCurvePointY += shiftY;
                toCurvePointX -= shiftX;

                fromTerminalPointY += shiftY;
                toTerminalPointX -= shiftX;

                midX1 = toCurvePointX;
                midY1 = fromCurvePointY;
            }

            // S-E
            if (FromTerminal.Port.Side == BlockSideEnum.South && ToTerminal.Port.Side == BlockSideEnum.East)
            {
                fromCurvePointX += shiftX;
                toCurvePointY -= shiftY;

                fromTerminalStartPointX += shiftX;
                toTerminalPointY -= shiftY;

                midX1 = fromTerminalStartPointX;
                midY1 = toCurvePointY;
            }

            // E-V
            if (FromTerminal.Port.Side == BlockSideEnum.East && ToTerminal.Port.Side == BlockSideEnum.West)
            {
                fromCurvePointY += shiftY;
                toCurvePointY += shiftY;

                fromTerminalPointY += shiftY;
                toTerminalPointY += shiftY;

                var xSpan = (fromCurvePointX - toCurvePointX);

                var ySpan = fromCurvePointY > toCurvePointY ? fromCurvePointY - toCurvePointY : toCurvePointY - fromCurvePointY;

                midX2 = toCurvePointX + (xSpan / 3);
                midX1 = toCurvePointX + ((xSpan / 3) * 2);

                if (fromCurvePointY > toCurvePointY)
                {
                    midY2 = toCurvePointY + (0.1 * ySpan);
                    midY1 = toCurvePointY + (0.9 * ySpan);
                }
                else
                {
                    midY2 = fromCurvePointY + (0.9 * ySpan);
                    midY1 = fromCurvePointY + (0.1 * ySpan);
                }
            }

            // V-E
            if (FromTerminal.Port.Side == BlockSideEnum.West && ToTerminal.Port.Side == BlockSideEnum.East)
            {
                fromCurvePointY += shiftY;
                toCurvePointY += shiftY;

                fromTerminalPointY += shiftY;
                toTerminalPointY += shiftY;

                var xSpan = (toCurvePointX - fromCurvePointX);

                var ySpan = fromCurvePointY > toCurvePointY ? fromCurvePointY - toCurvePointY : toCurvePointY - fromCurvePointY;

                midX1 = fromCurvePointX + (xSpan / 3);
                midX2 = fromCurvePointX + ((xSpan / 3) * 2);

                if (fromCurvePointY > toCurvePointY)
                {
                    midY1 = toCurvePointY + (0.9 * ySpan);
                    midY2 = toCurvePointY + (0.1 * ySpan);
                }
                else
                {
                    midY1 = fromCurvePointY + (0.1 * ySpan);
                    midY2 = fromCurvePointY + (0.9 * ySpan);
                }
            }


            // V-V
            if (FromTerminal.Port.Side == BlockSideEnum.West && ToTerminal.Port.Side == BlockSideEnum.West)
            {
                fromCurvePointY += shiftY;
                toCurvePointY += shiftY;

                fromTerminalPointY += shiftY;
                toTerminalPointY += shiftY;

                var ySpan = fromCurvePointY > toCurvePointY ? fromCurvePointY - toCurvePointY : toCurvePointY - fromCurvePointY;

                var canvasWith = _canvasEnvelope.Width * 10000;

                // If width of canvas is less than ySpan use 90% of canvas with - to prevent that curve is drawed outside canvas
                if (canvasWith < ySpan)
                    ySpan = canvasWith * 0.9;

                midX1 = fromCurvePointX + ySpan;
                midX2 = fromCurvePointX + ySpan;

                if (fromCurvePointY > toCurvePointY)
                {
                    midY1 = toCurvePointY + (0.9 * ySpan);
                    midY2 = toCurvePointY + (0.1 * ySpan);
                }
                else
                {
                    midY1 = fromCurvePointY + (0.1 * ySpan);
                    midY2 = fromCurvePointY + (0.9 * ySpan);
                }

                sameSide = true;
            }

            // E-E
            if (FromTerminal.Port.Side == BlockSideEnum.East && ToTerminal.Port.Side == BlockSideEnum.East)
            {
                fromCurvePointY += shiftY;
                toCurvePointY += shiftY;

                fromTerminalPointY += shiftY;
                toTerminalPointY += shiftY;

                var ySpan = fromCurvePointY > toCurvePointY ? fromCurvePointY - toCurvePointY : toCurvePointY - fromCurvePointY;

                var canvasWith = _canvasEnvelope.Width * 10000;

                // If width of canvas is less than ySpan use 90% of canvas with - to prevent that curve is drawed outside canvas
                if (canvasWith < ySpan)
                    ySpan = canvasWith * 0.9;

                midX1 = fromCurvePointX - ySpan;
                midX2 = fromCurvePointX - ySpan;

                if (fromCurvePointY > toCurvePointY)
                {
                    midY1 = toCurvePointY + (0.9 * ySpan);
                    midY2 = toCurvePointY + (0.1 * ySpan);
                }
                else
                {
                    midY1 = fromCurvePointY + (0.1 * ySpan);
                    midY2 = fromCurvePointY + (0.9 * ySpan);
                }

                sameSide = true;
            }

            // S-S
            if (FromTerminal.Port.Side == BlockSideEnum.South && ToTerminal.Port.Side == BlockSideEnum.South)
            {
                fromCurvePointY += shiftY;
                toCurvePointY += shiftY;

                fromTerminalPointY += shiftY;
                toTerminalPointY += shiftY;

                var xSpan = fromCurvePointX > toCurvePointX ? fromCurvePointX - toCurvePointX : toCurvePointX - fromCurvePointX;

                var canvasHeight = _canvasEnvelope.Height * 10000;

                // If hight of canvas is less than ySpan use 90% of canvas hight - to prevent that curve is drawed outside canvas
                if (canvasHeight < xSpan)
                    xSpan = canvasHeight * 0.9;

                midY1 = fromCurvePointY + xSpan;
                midY2 = fromCurvePointY + xSpan;

                if (fromCurvePointX > toCurvePointX)
                {
                    midX1 = toCurvePointX + (0.9 * xSpan);
                    midX2 = toCurvePointX + (0.1 * xSpan);
                }
                else
                {
                    midX1 = fromCurvePointX + (0.1 * xSpan);
                    midX2 = fromCurvePointX + (0.9 * xSpan);
                }

                sameSide = true;
            }

            // N-N
            if (FromTerminal.Port.Side == BlockSideEnum.North && ToTerminal.Port.Side == BlockSideEnum.North)
            {
                fromCurvePointY += shiftY;
                toCurvePointY += shiftY;

                fromTerminalPointY += shiftY;
                toTerminalPointY += shiftY;

                var xSpan = fromCurvePointX > toCurvePointX ? fromCurvePointX - toCurvePointX : toCurvePointX - fromCurvePointX;

                var canvasHeight = _canvasEnvelope.Height * 10000;

                // If hight of canvas is less than ySpan use 90% of canvas hight - to prevent that curve is drawed outside canvas
                if (canvasHeight < xSpan)
                    xSpan = canvasHeight * 0.9;

                midY1 = fromCurvePointY - xSpan;
                midY2 = fromCurvePointY - xSpan;

                if (fromCurvePointX > toCurvePointX)
                {
                    midX1 = toCurvePointX + (0.9 * xSpan);
                    midX2 = toCurvePointX + (0.1 * xSpan);
                }
                else
                {
                    midX1 = fromCurvePointX + (0.1 * xSpan);
                    midX2 = fromCurvePointX + (0.9 * xSpan);
                }

                sameSide = true;
            }



            LineString curve = null;

            // straigh line
            if (!sameSide && ((fromTerminalStartPointX == toTerminalPointX) || (fromTerminalPointY == toTerminalPointY)))
            {
                List<Coordinate> pnts = new List<Coordinate>();
                pnts.Add(new Coordinate(GeometryBuilder.Convert(fromTerminalStartPointX), GeometryBuilder.Convert(fromTerminalPointY)));
                pnts.Add(new Coordinate(GeometryBuilder.Convert(toTerminalPointX), GeometryBuilder.Convert(toTerminalPointY)));
                curve = new LineString(pnts.ToArray());
            }
            else
            {
                if (midX2 == -1)
                    curve = GeometryBuilder.Beizer(fromCurvePointX, fromCurvePointY, midX1, midY1, toCurvePointX, toCurvePointY, fromTerminalStartPointX, fromTerminalPointY, toTerminalPointX, toTerminalPointY);
                else
                    curve = GeometryBuilder.Beizer(fromCurvePointX, fromCurvePointY, midX1, midY1, midX2, midY2, toCurvePointX, toCurvePointY, fromTerminalStartPointX, fromTerminalPointY, toTerminalPointX, toTerminalPointY);
            }
            return curve;
        }
    }
}
