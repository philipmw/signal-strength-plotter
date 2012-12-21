using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SignalPlotter.Model.VerizonAppGateway;

namespace SignalPlotter.ViewModel
{
    class MainWindow : BaseViewModel
    {
        public struct SSHashes
        {
            public string sumHash, detHash;
        }
        HashSet<SSHashes> hashUnrecogSet = new HashSet<SSHashes>();
        ulong hashGood, hashUnrecog, hashUnavail;
        public ulong HashGoodInstances
        {
            get { return hashGood; }
            set
            {
                hashGood = value;
                NotifyPropertyChanged();
            }
        }
        public ulong HashUnrecogInstances
        {
            get { return hashUnrecog; }
            set
            {
                hashUnrecog = value;
                NotifyPropertyChanged();
            }
        }
        public ulong HashUnavailInstances
        {
            get { return hashUnavail; }
            set
            {
                hashUnavail = value;
                NotifyPropertyChanged();
            }
        }
        public long HashUnrecogUnique
        {
            get { return hashUnrecogSet.Count; }
        }

        void SampleAvailable(object sender, Model.Sample s)
        {
            if (s.sss.HasValue)
            {
                Screenshot.SignalStrengthSample sss = s.sss.Value;
                if (sss.ss.HasValue)
                {
                    Screenshot.SignalStrength ss = sss.ss.Value;

                    NetSel = ss.netChoice.ToString("g");
                    Bars4G = ss.bars4g.ToString();
                    Bars3G = ss.bars3g.ToString();
                    Bars2G = ss.bars2g.ToString();
                    ++HashGoodInstances;
                }
                else
                {
                    NetSel = "N/A";
                    Bars4G = "N/A";
                    Bars3G = "N/A";
                    Bars2G = "N/A";
                    Debug.WriteLine("Screenshot was unrecognized, but hashes are: sum=" + sss.sumHash + ", det=" + sss.detailHash);
                    hashUnrecogSet.Add(new SSHashes { sumHash = sss.sumHash, detHash = sss.detailHash });
                    ++HashUnrecogInstances;
                }
            }
            else
            {
                Debug.WriteLine("Screenshot could not be captured!");
                ++HashUnavailInstances;
            }

            GpsTime = s.gps.time.ToString();
            NotifyPropertyChanged("GpsTime");
            Latitude = s.gps.position.Latitude.ToString();
            NotifyPropertyChanged("Latitude");
            Longitude = s.gps.position.Longitude.ToString();
            NotifyPropertyChanged("Longitude");
            Elevation = s.gps.position.Altitude.ToFeet().ToString();
            NotifyPropertyChanged("Elevation");
            Speed = s.gps.speed5sec.ToImperialUnitType().ToString();
            NotifyPropertyChanged("Speed");
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
        }

        public MainWindow()
        {
            Model.MainThread.Instance.SampleAvailable += SampleAvailable;
        }

        public string GpsTime { get; private set; }
        public string Latitude { get; private set; }
        public string Longitude { get; private set; }
        public string Elevation { get; private set; }
        public string Speed { get; private set; }
        public string Satellites { get; private set; }

        string netSel;
        public string NetSel
        {
            get { return netSel; }
            private set
            {
                netSel = value;
                NotifyPropertyChanged();
            }
        }
        string bars4g, bars3g, bars2g;
        public string Bars4G
        {
            get { return bars4g; }
            private set
            {
                bars4g = value;
                NotifyPropertyChanged();
            }
        }
        public string Bars3G
        {
            get { return bars3g; }
            private set
            {
                bars3g = value;
                NotifyPropertyChanged();
            }
        }
        public string Bars2G
        {
            get { return bars2g; }
            private set
            {
                bars2g = value;
                NotifyPropertyChanged();
            }
        }

        public string PingEma { get; private set; }
        public string PingMin { get; private set; }
        public string PingMax { get; private set; }

        public UInt16 UnrecogScreens { get; private set; }
    }
}
