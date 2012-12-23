using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LatencyService.Model
{
    public class PingService : ILatencyServiceContract
    {
        static object syncRoot = new object();
        TimeSpan timeBetweenSamples = TimeSpan.FromSeconds(5);
        IPAddress[] hosts = new IPAddress[]
        {
            new IPAddress(new byte[] { 4, 2, 2, 2 } ), //well-known DNS server
            new IPAddress(new byte[] { 8, 8, 8, 8 } ), //Google's well-known DNS server
        };

        public event EventHandler NewResultAvailable;
        public IPAddress[] Hosts { get { return hosts; } }
        public TimeSpan PingPeriod { get { return timeBetweenSamples; } }
        public ushort MaxNumResults { get; private set; }

        // Exponential Moving Average
        long ema;
        ushort emaSamples;
        public LatencySample LatencyEMA
        {
            get
            {
                if (emaSamples < 2)
                    return new LatencySample { status = SampleStatus.Nonexistent };
                else
                    return new LatencySample { status = SampleStatus.Good, rttMs = ema };
            }
        }

        /// <summary>
        /// Results is an array of linked lists.  Each element of the array
        /// corresponds to a host in the 'hosts' array.  Each linked list stores
        /// the latency, in milliseconds, of each ping attempt.  If a ping
        /// attempt times out, the null value is stored.
        /// </summary>
        public LinkedList<long?>[] Results { get; private set; }
        public ulong[] FastestCount { get; private set; }
        public ulong TimedoutSamples { get; private set; }
        LatencySample latestMinimumSample = new LatencySample { status = SampleStatus.Nonexistent };

        Thread t;
        Ping ping;
        public PingService()
        {
            t = new Thread(ThreadRun);
            MaxNumResults = 5;
            Results = new LinkedList<long?>[hosts.Count()];
            FastestCount = new ulong[hosts.Count()];
            for (ushort i = 0; i < Results.Count(); ++i)
            {
                Results[i] = new LinkedList<long?>();
            }
        }

        public void Start()
        {
            if (t != null && t.IsAlive)
            {
                throw new Exception("You're trying to re-start an already running thread.");
            }
            ping = new Ping();
            t.Start();
        }

        void ThreadRun()
        {
            while (true) // abort will take care of this
            {
                long latencyMinMs = long.MaxValue;
                ushort? latencyMinIdx = null;

                // This loop pings each known host one time, stores the result,
                // and determines the lowest ping time.
                for (ushort i = 0; i < hosts.Count(); ++i)
                {
                    IPAddress host = hosts[i];
                    LinkedList<long?> results = Results[i];

                    const ushort MaxWaitMs = 2000;
                    try
                    {
                        PingReply reply = ping.Send(host, MaxWaitMs);
                        if (reply.Status != IPStatus.Success)
                        {
                            results.AddFirst((long?)null); // Must match #2
                        }
                        else
                        {
                            // Successful ping reply
                            results.AddFirst(reply.RoundtripTime);
                            if (latencyMinIdx == null || reply.RoundtripTime < latencyMinMs)
                            {
                                latencyMinMs = reply.RoundtripTime;
                                latencyMinIdx = i;
                            }
                        }
                    }
                    catch (PingException)
                    {
                        results.AddFirst((long?)null); // Must match #1
                    }

                    if (results.Count > MaxNumResults)
                        results.RemoveLast();
                } // end of for-loop pinging every host

                // Now we know which host, if any, is fastest.

                if (latencyMinIdx != null)
                {
                    ++FastestCount[latencyMinIdx.Value];

                    latestMinimumSample.status = SampleStatus.Good;
                    latestMinimumSample.rttMs = latencyMinMs;

                    // Update EMA
                    ushort numEmaSamples = Math.Min(MaxNumResults, ++emaSamples);
                    double alpha = 2.0 / (numEmaSamples + 1); // http://en.wikipedia.org/wiki/Moving_average#Exponential_moving_average
                    ema = (ushort)Math.Round((latencyMinMs * alpha) + (ema * (1 - alpha)));
                    Debug.WriteLine("EMA of "+numEmaSamples+" samples: " + ema);
                }
                else
                {
                    ++TimedoutSamples;

                    latestMinimumSample.status = SampleStatus.TimedOut;

                    // Update EMA
                    emaSamples = 0;
                }

                if (NewResultAvailable != null)
                    NewResultAvailable(this, null);

                Thread.Sleep(timeBetweenSamples);
            }
        }

        public void Stop()
        {
            t.Abort();
            t.Join();
        }

        public LatestData LatestLatency()
        {
            LatestData d = new LatestData();
            lock (syncRoot)
            {
                d.ema = LatencyEMA;
                d.latest = latestMinimumSample;
            }
            return d;
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
