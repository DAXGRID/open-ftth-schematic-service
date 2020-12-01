using NetTopologySuite.Geometries;
using OpenFTTH.Schematic.Business.Lines;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenFTTH.Schematic.Business.Drawing
{
    public static class GeometryBuilder
    {
        public static Polygon Rectangle(double offsetX, double offsetY, double height, double width)
        {
            var coordinates = new Coordinate[5];

            coordinates[0] = new Coordinate(Convert(offsetX), Convert(offsetY));
            coordinates[1] = new Coordinate(Convert(offsetX + width), Convert(offsetY));
            coordinates[2] = new Coordinate(Convert(offsetX + width), Convert(offsetY + height));
            coordinates[3] = new Coordinate(Convert(offsetX), Convert(offsetY + height));
            coordinates[4] = new Coordinate(Convert(offsetX), Convert(offsetY));

            var ring = new LinearRing(coordinates);

            return new Polygon(ring);
        }

        public static Point Point(double x, double y)
        {
            return new Point(Convert(x), Convert(y));
        }

        public static LineString Beizer(double curveStartX, double curveStartY, double viaX, double viaY, double curveEndX, double curveEndY, double startX, double startY, double endX, double endY)
        {
            int nPoints = 50;

            double[] ps = new double[6];

            ps[0] = curveStartX;
            ps[1] = curveStartY;
            ps[2] = viaX;
            ps[3] = viaY;
            ps[4] = curveEndX;
            ps[5] = curveEndY;

            double[] result = new double[nPoints * 2];

            var bc = new BezierCurve();
            bc.Bezier2D(ps, nPoints, result);

            Coordinate[] pnts = new Coordinate[nPoints + 2];

            // Add first point
            pnts[0] = new Coordinate(Convert(startX), Convert(startY));

            var pntIndex = 1;

            for (int i = 0; i < ((nPoints * 2) - 1); i+=2)
            {
                pnts[pntIndex] = new Coordinate(Convert(result[i]), Convert(result[i + 1]));
                pntIndex++;
            }

            // Add last point
            pnts[nPoints+1] = new Coordinate(Convert(endX), Convert(endY));


            return new LineString(pnts);
        }

        /// <summary>
        /// Convert from mm to wgs84 valid coordinate
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double Convert(double value)
        {
            //return value;
            return value / 10000;
        }

        
    }
}
