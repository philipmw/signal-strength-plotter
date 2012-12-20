using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SignalPlotter.Model.VerizonAppGateway;
using System.IO;
using System.Diagnostics;

namespace SignalPlotter.ViewModel
{
    class MainWindow : BaseViewModel
    {
        void SampleAvailable(object sender, Model.Sample s)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(s.gps.time.ToString() + ",");
            sb.Append(s.gps.satellites + ",");
            sb.Append(s.gps.position.Latitude.DecimalDegrees + ",");
            sb.Append(s.gps.position.Longitude.DecimalDegrees + ",");
            sb.Append(s.gps.position.Altitude.ToMeters().Value + ",");

            if (s.ss.HasValue)
            {
                Screenshot.SignalStrength ss = s.ss.Value;

                sb.Append(ss.netChoice.ToString("g") + ",");
                sb.Append(ss.bars4g + ",");
                sb.Append(ss.bars3g + ",");
                sb.Append(ss.bars2g + ",");

                NetSel = ss.netChoice.ToString("g");
                Bars4G = ss.bars4g;
                Bars3G = ss.bars3g;
                Bars2G = ss.bars2g;
                NotifyPropertyChanged("NetSel");
                NotifyPropertyChanged("Bars4G");
                NotifyPropertyChanged("Bars3G");
                NotifyPropertyChanged("Bars2G");
            }
            else
            {
                Debug.WriteLine("Got a sample, but Screenshot.SignalStrength is unavailable!");
                sb.Append("no-signal-info,,,,");
                ++UnrecogScreens;
                NotifyPropertyChanged("UnrecogScreens");
            }

            GpsTime = s.gps.time.ToString();
            NotifyPropertyChanged("GpsTime");
            Latitude = s.gps.position.Latitude.ToString();
            NotifyPropertyChanged("Latitude");
            Longitude = s.gps.position.Longitude.ToString();
            NotifyPropertyChanged("Longitude");
            Altitude = s.gps.position.Altitude.ToFeet().ToString();
            NotifyPropertyChanged("Altitude");
            Satellites = s.gps.satellites.ToString();
            NotifyPropertyChanged("Satellites");

            PingEma = s.latency.ema.ToString();
            NotifyPropertyChanged("PingEma");
            if (s.latency.minLatency.HasValue)
                PingMin = s.latency.minLatency.ToString();
            else
                PingMin = "N/A";
            NotifyPropertyChanged("PingMin");
            if (s.latency.maxLatency.HasValue)
                PingMax = s.latency.maxLatency.ToString();
            else
                PingMax = "N/A";
            NotifyPropertyChanged("PingMax");

            sb.Append(PingEma + ",");
            sb.Append(PingMin + ",");
            sb.Append(PingMax);

            dataStream.WriteLine(sb);
            dataStream.FlushAsync();
        }

        StreamWriter dataStream;

        public MainWindow()
        {
            Model.MainThread.Instance.SampleAvailable += SampleAvailable;
            string outputFilename = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\Desktop\\vz-output.txt";
            dataStream = File.AppendText(outputFilename);
            dataStream.WriteLine("Opened for appending at " + DateTime.Now);
        }

        public string GpsTime { get; private set; }
        public string Latitude { get; private set; }
        public string Longitude { get; private set; }
        public string Altitude { get; private set; }
        public string Satellites { get; private set; }

        public string NetSel { get; private set; }
        public UInt16 Bars4G { get; private set; }
        public UInt16 Bars3G { get; private set; }
        public UInt16 Bars2G { get; private set; }

        public string PingEma { get; private set; }
        public string PingMin { get; private set; }
        public string PingMax { get; private set; }

        public UInt16 UnrecogScreens { get; private set; }
    }
}
