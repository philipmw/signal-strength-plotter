using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SignalPlotter.Model.VerizonAppGateway;

namespace SignalPlotter.ViewModel
{
    class MainWindow : BaseViewModel
    {
        void notifyGpsUpdates()
        {
            NotifyPropertyChanged("GpsTime");
        }

        void SignalStrengthAvailable(Screenshot.SignalStrength? ssOrNull)
        {
            if (ssOrNull.HasValue)
            {
                Screenshot.SignalStrength ss = ssOrNull.Value;
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
                ++UnrecogScreens;
                NotifyPropertyChanged("UnrecogScreens");
            }
        }

        void GpsInfoAvailable(string timestamp)
        {
            GpsTime = timestamp;
            notifyGpsUpdates();
        }

        void LatencyAvailable(long? latency)
        {
            if (latency.HasValue)
            {
                PingMs = latency.Value.ToString();
            }
            else
            {
                PingMs = "N/A";
            }
            NotifyPropertyChanged("PingMs");
        }

        public MainWindow()
        {
            Model.MainThread.Instance.SignalStrengthAvailable += SignalStrengthAvailable;
            Model.MainThread.Instance.GpsInfoAvailable += GpsInfoAvailable;
            Model.MainThread.Instance.LatencyAvailable += LatencyAvailable;
        }

        public string GpsTime { get; private set; }
        public string Latitude { get; private set; }
        public string Longitude { get; private set; }

        public string NetSel { get; private set; }
        public UInt16 Bars4G { get; private set; }
        public UInt16 Bars3G { get; private set; }
        public UInt16 Bars2G { get; private set; }

        public string PingMs { get; private set; }

        public UInt16 UnrecogScreens { get; private set; }
    }
}
