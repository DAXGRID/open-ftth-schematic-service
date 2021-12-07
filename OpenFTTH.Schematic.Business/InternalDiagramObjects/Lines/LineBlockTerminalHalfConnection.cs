using NetTopologySuite.Geometries;
using OpenFTTH.Schematic.API.Model.DiagramLayout;
using OpenFTTH.Schematic.Business.Drawing;
using System;
using System.Collections.Generic;

namespace OpenFTTH.Schematic.Business.Lines
{
    public class LineBlockTerminalHalfConnection
    {
        public BlockPortTerminal FromTerminal { get; set; }
        public string Label { get; set; }
        public string Style { get; set; }
        public int DrawingOrder { get; set; }

        public double LineLength { get; set; }

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


            NetTopologySuite.Geometries.LineString curve = CreateCurve(0, 0, 0);

            result.Add(new DiagramObject(diagram)
            {
                IdentifiedObject = _refClass == null ? null : new IdentifiedObjectReference() { RefId = _refId, RefClass = _refClass },
                Style = Style is null ? "Cable" : Style,
                Label = this.Label,
                Geometry = curve,
                DrawingOrder = DrawingOrder
            });

            return result;
        }

        private NetTopologySuite.Geometries.LineString CreateCurve(double shiftX = 0, double shiftY = 0, double extraLengthBothEnds = 0)
        {
            double fromTerminalStartPointX = FromTerminal.ConnectionPointX;
            double fromTerminalStartPointY = FromTerminal.ConnectionPointY;

            double toTerminalEndPointX = FromTerminal.ConnectionPointX;
            double toTerminalEndPointY = FromTerminal.ConnectionPointY;

            // Calculate from curve point
            if (FromTerminal.Port.Side == BlockSideEnum.West)
            {
                toTerminalEndPointX += LineLength;
            }

            if (FromTerminal.Port.Side == BlockSideEnum.North)
            {
                toTerminalEndPointY -= LineLength;
            }

            if (FromTerminal.Port.Side == BlockSideEnum.East)
            {
                toTerminalEndPointX -= LineLength;
            }

            if (FromTerminal.Port.Side == BlockSideEnum.South)
            {
                toTerminalEndPointY += LineLength;
            }
            
            List<Coordinate> pnts = new List<Coordinate>();
            pnts.Add(new Coordinate(GeometryBuilder.Convert(fromTerminalStartPointX), GeometryBuilder.Convert(fromTerminalStartPointY)));
            pnts.Add(new Coordinate(GeometryBuilder.Convert(toTerminalEndPointX), GeometryBuilder.Convert(toTerminalEndPointY)));

            return new LineString(pnts.ToArray());
        }
    }
}
