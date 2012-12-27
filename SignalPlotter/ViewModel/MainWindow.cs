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
            UpdateSignalElements(s);
            UpdateGpsElements(s.gps);
            UpdateLatencyElements(s.latency);
        }

        void UpdateSignalElements(Model.Sample s)
        {
            if (s.ssss == Model.SignalStrengthSampleStatus.Normal)
            {
                Screenshot.SignalStrengthSample sss = s.sss.Value;
                if (sss.netChoice.HasValue)
                {
                    NetSel = sss.netChoice.Value.ToString("g");
                }
                else
                {
                    NetSel = "N/A";
                }

                if (sss.detail.HasValue)
                {
                    Screenshot.SignalDetail sd = sss.detail.Value;
                    ConnectedTo = sd.onWWAN;
                    Bars4G = sd.bars4g.ToString();
                    Bars3G = sd.bars3g.ToString();
                    Bars2G = sd.bars2g.ToString();
                }
                else
                {
                    ConnectedTo = null;
                    NetSel = "N/A";
                    Bars4G = "N/A";
                    Bars3G = "N/A";
                    Bars2G = "N/A";
                }

                if (sss.netChoice.HasValue && sss.detail.HasValue)
                {
                    ++HashGoodInstances;
                }
                else
                {
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
        }

        void UpdateGpsElements(PmwGpsService.LatestGpsData? dataOrNull)
        {
            if (dataOrNull.HasValue)
            {
                PmwGpsService.LatestGpsData gps = dataOrNull.Value;
                GpsTime = gps.time.ToString();
                Latitude = gps.position.Latitude.ToString();
                Longitude = gps.position.Longitude.ToString();
                Elevation = gps.position.Altitude.ToFeet().ToString();
                Speed = gps.speed5sec.ToImperialUnitType().ToString();
                Satellites = gps.satellites.ToString();
            }
            else
            {
                GpsTime = "N/A";
                Latitude = "N/A";
                Longitude = "N/A";
                Elevation = "N/A";
                Speed = "N/A";
                Satellites = "N/A";
            }
        }

        void UpdateLatencyElements(PmwLatencyService.LatestData? dataOrNull)
        {
            if (dataOrNull.HasValue)
            {
                PmwLatencyService.LatestData ld = dataOrNull.Value;
                PingEma = ld.ema;
                PingLatest = ld.latest;
            }
            else
            {
                PingEma = new PmwLatencyService.LatencySample { status = PmwLatencyService.SampleStatus.Nonexistent };
                PingLatest = new PmwLatencyService.LatencySample { status = PmwLatencyService.SampleStatus.Nonexistent };
            }

        }

        public MainWindow()
        {
            Model.MainThread.Instance.SampleAvailable += SampleAvailable;
        }

        // *** Start of GPS-specific Properties ***
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
        // *** End of GPS-specific Properties ***

        // *** Start of Connectivity-specific Properties ***
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

        Screenshot.ConnectedTo? connectedTo;
        public Screenshot.ConnectedTo? ConnectedTo
        {
            get { return connectedTo; }
            private set
            {
                connectedTo = value;
                NotifyPropertyChanged();
            }
        }
        // *** End of Connectivity-specific Properties ***

        // *** Start of Latency-specific Properties ***
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
        // *** End of Latency-specific Properties ***
    }
}
