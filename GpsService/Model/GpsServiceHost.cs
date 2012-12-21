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
            s.position = new DotSpatial.Positioning.Position3D(t.Elevation, t.GpsPos);
            s.speed5sec = t.Speed5sec;
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

        public DotSpatial.Positioning.Distance Elevation()
        {
            GpsThread t = ((App)Application.Current).gpsThread;
            t.NotifyOfExternalRequest();
            return t.Elevation;
        }

        public DotSpatial.Positioning.Speed Speed5Sec()
        {
            GpsThread t = ((App)Application.Current).gpsThread;
            t.NotifyOfExternalRequest();
            return t.Speed5sec;            
        }

        public ushort Satellites()
        {
            GpsThread t = ((App)Application.Current).gpsThread;
            t.NotifyOfExternalRequest();
            return t.Satellites;
        }
    }
}
