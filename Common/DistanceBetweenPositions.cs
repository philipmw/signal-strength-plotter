using DotSpatial.Positioning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class Position3DExtension
    {
        static double DegToRad(double deg)
        {
            return deg * (2 * Math.PI) / 360;
        }

        public static Distance DistanceFromOnEarth(this Position3D s, Position3D f)
        {
            const double RadiusEarthKm = 6378.1; // according to Google
            double radius = RadiusEarthKm +
                ((s.Altitude.ToKilometers().Value + f.Altitude.ToKilometers().Value) / 2);
            var latDist = f.Latitude - s.Latitude;
            var lonDist = f.Longitude - s.Longitude;
            var eleDist = f.Altitude - s.Altitude;
            double distInDeg = Math.Sqrt(Math.Pow(latDist.DecimalDegrees, 2) + Math.Pow(lonDist.DecimalDegrees, 2));
            //Debug.WriteLine("2D distance in degrees: " + distInDeg);
            Distance dist2d = new Distance(radius * Math.Tan(DegToRad(distInDeg)), DistanceUnit.Kilometers);
            return dist2d + eleDist;
        }
    }
}
