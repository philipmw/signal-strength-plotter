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
        Model.PingService ps = ((App)Application.Current).pingService;

        public TimeSpan PingPeriod { get { return ps.PingPeriod; } }
        public ushort EMA { get { return ps.LatencyEMA; } }
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

        string host1name;
        public string Host1name
        {
            get { return host1name; }
            private set {
                host1name = value;
                NotifyPropertyChanged();
            }
        }
        string host2name;
        public string Host2name
        {
            get { return host2name; }
            private set
            {
                host2name = value;
                NotifyPropertyChanged();
            }
        }

        public string Host1t0 { get { return PingResult(0, 0); } }
        public string Host1t1 { get { return PingResult(0, 1); } }
        public string Host1t2 { get { return PingResult(0, 2); } }
        public string Host1t3 { get { return PingResult(0, 3); } }
        public string Host1t4 { get { return PingResult(0, 4); } }

        public string Host2t0 { get { return PingResult(1, 0); } }
        public string Host2t1 { get { return PingResult(1, 1); } }
        public string Host2t2 { get { return PingResult(1, 2); } }
        public string Host2t3 { get { return PingResult(1, 3); } }
        public string Host2t4 { get { return PingResult(1, 4); } }

        string PingResult(int hostIndex, int tIndex)
        {
            if (ps.Results != null && ps.Results[hostIndex].Count > tIndex)
            {
                long? e = ps.Results[hostIndex].ElementAt(tIndex);
                if (e.HasValue)
                    return e.Value.ToString();
                else
                    return "###";
            }
            else
            {
                return "--";
            }
        }

        public MainWindow()
        {
        }

        public void Start()
        {
            ps.NewResultAvailable += NewResultAvailable;
            ps.RequestProcessed += ExtReqsUpdated;
            Host1name = ps.Hosts[0].ToString();
            Host2name = ps.Hosts[1].ToString(); // yes, this assumes there are at least two hosts defined.  If you crashed here, ask yourself why you don't have at least two defined.
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
