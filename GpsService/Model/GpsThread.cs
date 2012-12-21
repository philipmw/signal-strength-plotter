using DotSpatial.Positioning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GpsService.Model
{
    class GpsThreadException : Exception
    {
        public GpsThreadException(string msg)
        : base(msg)
        {
        }
    }

    public class GpsThread : IDisposable
    {
        Device dev;
        NmeaInterpreter nmea;
        Thread t;
        object syncRoot = new object();

        public delegate void GpsDataUpdatedDelegate();
        public event GpsDataUpdatedDelegate GpsDataUpdated;

        public GpsThread()
        {
            Devices.DeviceDetected += DeviceDetected;
            Devices.AllowExhaustiveSerialPortScanning = false;
            Devices.AllowBluetoothConnections = false;
            Devices.BeginDetection();
            Devices.WaitForDetection();
            if (dev == null)
            {
                throw new GpsThreadException("Did not find any GPS devices!");
            }

            nmea = new NmeaInterpreter();
            nmea.SatellitesChanged += SatellitesChanged;
            nmea.DateTimeChanged += TimeChanged;
            nmea.PositionChanged += PositionChanged;
            nmea.AltitudeChanged += ElevationChanged;
            nmea.Start(dev);
            t = new Thread(ThreadRun);
            t.Start();
        }

        public string DevName { get { return dev.Name; } }
        public ushort Satellites { get; private set; }
        public Position GpsPos { get; private set; }
        public Latitude Lat { get { return GpsPos.Latitude; } }
        public Longitude Lon { get { return GpsPos.Longitude; } }
        public Distance Elevation { get; private set; }
        public Speed Speed5sec { get; private set; }
        public DateTime GpsTime { get; private set; }
        public bool IsConnected { get { return dev.IsOpen; } }

        void DeviceDetected(object sender, DeviceEventArgs e)
        {
            dev = e.Device;
            Debug.WriteLine("Detected GPS device: " + dev.Name);
        }

        void SatellitesChanged(object sender, SatelliteListEventArgs e)
        {
            lock (syncRoot)
            {
                Satellites = (ushort)e.Satellites.Count;
            }
            Debug.WriteLine("Satellites changed: " + Satellites);
            NotifyOfGpsDataUpdate();
        }

        void TimeChanged(object sender, DateTimeEventArgs e)
        {
            lock (syncRoot)
            {
                GpsTime = e.DateTime;
            }
            Debug.WriteLine("Time changed: " + GpsTime.ToString());
            NotifyOfGpsDataUpdate();
        }

        void PositionChanged(object sender, PositionEventArgs e)
        {
            lock (syncRoot)
            {
                GpsPos = e.Position;
                // We don't receive elevation and position simultaneously, so
                // take the last known elevation.
                AddPosSample(new Position3D(Elevation, e.Position));
            }
            Debug.WriteLine("Position changed: " + GpsPos.ToString());
            NotifyOfGpsDataUpdate();
        }

        struct TimestampedPosition
        {
            public DateTime ts;
            public Position3D pos;
        }
        LinkedList<TimestampedPosition> posHist = new LinkedList<TimestampedPosition>();
        void AddPosSample(Position3D p)
        {
            posHist.AddLast(new TimestampedPosition { ts = DateTime.Now, pos = p });
            while (posHist.Count > 0 && (DateTime.Now - posHist.First.Value.ts) > TimeSpan.FromSeconds(5))
            {
                posHist.RemoveFirst();
            }

            if (posHist.Count > 1)
            {
                Position3D startPos = posHist.First.Value.pos;
                Position3D endPos = posHist.Last.Value.pos;
                double seconds = (posHist.Last.Value.ts - posHist.First.Value.ts).TotalSeconds;
                Distance d = FindDistanceOnEarth(startPos, endPos);
                Debug.WriteLine("Seconds elapsed="+seconds+", distance="+d.ToImperialUnitType().ToString());
                Speed5sec = new Speed(d.Value / seconds, SpeedUnit.KilometersPerSecond);
            }
        }

        static Distance FindDistanceOnEarth(Position3D s, Position3D f)
        {
            const double RadiusEarthKm = 6378.1; // according to Google
            double radius = RadiusEarthKm +
                ((s.Altitude.ToKilometers().Value + f.Altitude.ToKilometers().Value)/2);
            var latDist = f.Latitude - s.Latitude;
            var lonDist = f.Longitude - s.Longitude;
            double distInDeg = Math.Sqrt(Math.Pow(latDist.DecimalDegrees, 2) + Math.Pow(lonDist.DecimalDegrees, 2));
            Debug.WriteLine("Distance in degrees: " + distInDeg);
            return new Distance(radius * Math.Tan((distInDeg/360) * (2*Math.PI)), DistanceUnit.Kilometers);
        }

        void ElevationChanged(object sender, DistanceEventArgs e)
        {
            lock (syncRoot)
            {
                Elevation = e.Distance;
            }
            Debug.WriteLine("Altitude changed: " + Elevation.ToString());
            NotifyOfGpsDataUpdate();
        }

        void NotifyOfGpsDataUpdate()
        {
            if (GpsDataUpdated != null)
            {
                GpsDataUpdated();
            }
        }

        ~GpsThread()
        {
            Dispose();
        }

        void ThreadRun()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    UpdateExternalRequestCount();
                }
            }
            catch (ThreadAbortException)
            {
                Debug.WriteLine("GpsThread caught ThreadAbort.  Bye!");
            }
        }

        public void Dispose()
        {
            if (dev != null)
            {
                Debug.WriteLine("Disposing of GpsThread");
                t.Abort();
                t.Join();
                dev.Close();
                dev = null;
            }
        }

        // == Code specific to the TimeBasedCounter ==

        TimeBasedCounter tbc = new TimeBasedCounter(TimeSpan.FromMinutes(1));
        public class RequestProcessedEventArgs : System.EventArgs
        {
            public readonly int requestsInLastMinute;
            public RequestProcessedEventArgs(int r)
            {
                requestsInLastMinute = r;
            }
        }
        public event EventHandler<RequestProcessedEventArgs> RequestProcessed;

        void UpdateExternalRequestCount()
        {
            if (tbc.Recount() && RequestProcessed != null)
            {
                RequestProcessed(this, new RequestProcessedEventArgs(tbc.Count()));
            }
        }

        public void NotifyOfExternalRequest()
        {
            tbc.Add();
            if (RequestProcessed != null)
            {
                RequestProcessed(this, new RequestProcessedEventArgs(tbc.Count()));
            }
        }
    }
}
