using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GpsService.Model
{
    public class GpsServiceHost : IGpsServiceContract
    {
        public bool IsConnected()
        {
            GpsThread t = ((App)Application.Current).gpsThread;
            t.NotifyOfExternalRequest();
            return t.IsConnected;
        }

        public LatestGpsData GetLatest()
        {
            GpsThread t = ((App)Application.Current).gpsThread;
            LatestGpsData s;
            s.position = new DotSpatial.Positioning.Position3D(t.Altitude, t.GpsPos);
            s.satellites = t.Satellites;
            s.time = t.GpsTime;
            t.NotifyOfExternalRequest();
            return s;
        }

        public DateTime GpsTime()
        {
            GpsThread t = ((App)Application.Current).gpsThread;
            t.NotifyOfExternalRequest();
            return t.GpsTime;
        }

        public DotSpatial.Positioning.Position GpsPosition()
        {
            GpsThread t = ((App)Application.Current).gpsThread;
            t.NotifyOfExternalRequest();
            return t.GpsPos;
        }

        public DotSpatial.Positioning.Distance Altitude()
        {
            GpsThread t = ((App)Application.Current).gpsThread;
            t.NotifyOfExternalRequest();
            return t.Altitude;
        }

        public ushort Satellites()
        {
            GpsThread t = ((App)Application.Current).gpsThread;
            t.NotifyOfExternalRequest();
            return t.Satellites;
        }
    }
}
