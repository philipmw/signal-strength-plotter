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
            nmea.AltitudeChanged += AltitudeChanged;
            nmea.Start(dev);
            t = new Thread(ThreadRun);
            t.Start();
        }

        public string DevName { get { return dev.Name; } }
        public ushort Satellites { get; private set; }
        public Position GpsPos { get; private set; }
        public Latitude Lat { get { return GpsPos.Latitude; } }
        public Longitude Lon { get { return GpsPos.Longitude; } }
        public Distance Altitude { get; private set; }
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
            }
            Debug.WriteLine("Position changed: " + GpsPos.ToString());
            NotifyOfGpsDataUpdate();
        }

        void AltitudeChanged(object sender, DistanceEventArgs e)
        {
            lock (syncRoot)
            {
                Altitude = e.Distance;
            }
            Debug.WriteLine("Altitude changed: " + Altitude.ToString());
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
