using LatencyService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LatencyService.ViewModel
{
    class MainWindow : BaseViewModel
    {
        PingService ps = ((App)Application.Current).pingService;

        public TimeSpan PingPeriod { get { return ps.PingPeriod; } }
        public long EMA { get { return ps.LatencyEMA; } }
        public ushort extReqs;
        public ushort ExtReqs
        {
            get { return extReqs; }
            private set
            {
                extReqs = value;
                NotifyPropertyChanged();
            }
        }
        public string Alert { get { return "---"; } }

        string host1name, host2name;
        public string Host1name
        {
            get { return host1name; }
            private set {
                host1name = value;
                NotifyPropertyChanged();
            }
        }
        public string Host2name
        {
            get { return host2name; }
            private set
            {
                host2name = value;
                NotifyPropertyChanged();
            }
        }

        public ulong Host1fastest
        {
            get { return ps.FastestCount[0]; }
        }
        public ulong Host2fastest
        {
            get { return ps.FastestCount[1]; }
        }

        public LatencySample Host1t0 { get { return PingResult(0, 0); } }
        public LatencySample Host1t1 { get { return PingResult(0, 1); } }
        public LatencySample Host1t2 { get { return PingResult(0, 2); } }
        public LatencySample Host1t3 { get { return PingResult(0, 3); } }
        public LatencySample Host1t4 { get { return PingResult(0, 4); } }

        public LatencySample Host2t0 { get { return PingResult(1, 0); } }
        public LatencySample Host2t1 { get { return PingResult(1, 1); } }
        public LatencySample Host2t2 { get { return PingResult(1, 2); } }
        public LatencySample Host2t3 { get { return PingResult(1, 3); } }
        public LatencySample Host2t4 { get { return PingResult(1, 4); } }

        LatencySample PingResult(int hostIndex, int tIndex)
        {
            LatencySample s;
            if (ps.Results != null && ps.Results[hostIndex].Count > tIndex)
            {
                long? e = ps.Results[hostIndex].ElementAt(tIndex);
                if (e.HasValue)
                {
                    s.status = SampleStatus.Good;
                    s.rttMs = e.Value;
                }
                else
                {
                    s.status = SampleStatus.TimedOut;
                    s.rttMs = -1;
                }
            }
            else
            {
                s.status = SampleStatus.Nonexistent;
                s.rttMs = -1;
            }
            return s;
        }

        public MainWindow()
        {
        }

        public void Start()
        {
            ps.NewResultAvailable += NewResultAvailable;
            ps.RequestProcessed += ExtReqsUpdated;
            Host1name = ps.Hosts[0].ToString();
            Host2name = ps.Hosts[1].ToString();
            ps.Start();
        }

        public void Stop()
        {
            ps.Stop();
        }

        void ExtReqsUpdated(object sender, Model.PingService.RequestProcessedEventArgs e)
        {
            ExtReqs = (ushort)e.requestsInLastMinute;
        }

        void NewResultAvailable(object sender, EventArgs e)
        {
            NotifyPropertyChanged("EMA");

            NotifyPropertyChanged("Host1fastest");
            NotifyPropertyChanged("Host2fastest");

            NotifyPropertyChanged("Host1t0");
            NotifyPropertyChanged("Host1t1");
            NotifyPropertyChanged("Host1t2");
            NotifyPropertyChanged("Host1t3");
            NotifyPropertyChanged("Host1t4");

            NotifyPropertyChanged("Host2t0");
            NotifyPropertyChanged("Host2t1");
            NotifyPropertyChanged("Host2t2");
            NotifyPropertyChanged("Host2t3");
            NotifyPropertyChanged("Host2t4");
        }
    }
}
