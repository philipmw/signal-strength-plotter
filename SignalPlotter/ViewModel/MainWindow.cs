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
        // *** Start of variables tracking screenshot samples ***
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

        ulong sampleNotCollectedInstances;
        public ulong SampleNotCollectedInstances
        {
            get { return sampleNotCollectedInstances; }
            private set
            {
                sampleNotCollectedInstances = value;
                NotifyPropertyChanged();
            }
        }
        // *** End of variables tracking screenshot samples ***

        void SampleAvailable(object sender, Model.Sample s)
        {
            if (s.ssss == Model.SignalStrengthSampleStatus.Normal)
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
                    NotifyPropertyChanged("HashUnrecogUnique");
                    ++HashUnrecogInstances;
                }
            }
            else if (s.ssss == Model.SignalStrengthSampleStatus.TooProximateToPrev)
            {
                ++SampleNotCollectedInstances;
            }
            else
            {
                throw new Exception("Unrecognized screenshot status!");
            }

            UpdateGpsElements(s);

            if (s.latency.HasValue)
            {
                PingEma = s.latency.Value.ema;
                PingLatest = s.latency.Value.latest;
            }
            else
            {
                PingEma = new PmwLatencyService.LatencySample { status = PmwLatencyService.SampleStatus.Nonexistent };
                PingLatest = new PmwLatencyService.LatencySample { status = PmwLatencyService.SampleStatus.Nonexistent };
            }
        }

        void UpdateGpsElements(Model.Sample s)
        {
            PmwGpsService.LatestGpsData gps = s.gps.Value;
            GpsTime = gps.time.ToString();
            Latitude = gps.position.Latitude.ToString();
            Longitude = gps.position.Longitude.ToString();
            Elevation = gps.position.Altitude.ToFeet().ToString();
            Speed = gps.speed5sec.ToImperialUnitType().ToString();
            Satellites = gps.satellites.ToString();
        }

        public MainWindow()
        {
            Model.MainThread.Instance.SampleAvailable += SampleAvailable;
        }

        string gpsTime, latitude, longitude, elevation, speed, satellites;
        public string GpsTime
        {
            get { return gpsTime; }
            private set
            {
                gpsTime = value;
                NotifyPropertyChanged();
            }
        }
        public string Latitude
        {
            get { return latitude; }
            private set
            {
                latitude = value;
                NotifyPropertyChanged();
            }
        }
        public string Longitude
        {
            get { return longitude; }
            private set
            {
                longitude = value;
                NotifyPropertyChanged();
            }
        }
        public string Elevation
        {
            get { return elevation; }
            private set
            {
                elevation = value;
                NotifyPropertyChanged();
            }
        }
        public string Speed
        {
            get { return speed; }
            private set
            {
                speed = value;
                NotifyPropertyChanged();
            }
        }
        public string Satellites
        {
            get { return satellites; }
            private set
            {
                satellites = value;
                NotifyPropertyChanged();
            }
        }

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

        PmwLatencyService.LatencySample pingEma;
        public PmwLatencyService.LatencySample PingEma
        {
            get { return pingEma; }
            private set
            {
                pingEma = value;
                NotifyPropertyChanged();
            }
        }

        PmwLatencyService.LatencySample pingLatest;
        public PmwLatencyService.LatencySample PingLatest
        {
            get { return pingLatest; }
            private set
            {
                pingLatest = value;
                NotifyPropertyChanged();
            }
        }
    }
}
