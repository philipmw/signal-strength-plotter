﻿using System;
using System.Collections.Generic;
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
        public long LatencyEMA { get; private set; }

        /// <summary>
        /// Results is an array of linked lists.  Each element of the array
        /// corresponds to a host in the 'hosts' array.  Each linked list stores
        /// the latency, in milliseconds, of each ping attempt.  If a ping
        /// attempt times out, the null value is stored.
        /// </summary>
        public LinkedList<long?>[] Results { get; private set; }
        public ulong[] FastestCount { get; private set; }

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
                ushort latencyMinIdx = 0;
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
                            results.AddFirst(reply.RoundtripTime);
                            if (reply.RoundtripTime < latencyMinMs)
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
                }
                ++FastestCount[latencyMinIdx];
                double alpha = 2.0 / (MaxNumResults + 1); // http://en.wikipedia.org/wiki/Moving_average#Exponential_moving_average
                LatencyEMA = (ushort)Math.Round((LatencyEMA * alpha) + (latencyMinMs * (1-alpha)));

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

        public long Ema()
        {
            lock (syncRoot)
            {
                return LatencyEMA;
            }
        }

        public LatestData LatestLatency()
        {
            LatestData d = new LatestData();
            lock (syncRoot)
            {
                d.ema = LatencyEMA;

                foreach (LinkedList<long?> hostResults in Results)
                {
                    foreach (var result in hostResults)
                    {
                        if (result.HasValue)
                        {
                            if (d.minLatency.HasValue)
                            {
                                if (result.Value < d.minLatency)
                                    d.minLatency = result;
                                // otherwise keep previous value
                            }
                            else
                            {
                                d.minLatency = result.Value;
                            }

                            if (d.maxLatency.HasValue)
                            {
                                if (result.Value > d.maxLatency)
                                    d.maxLatency = result;
                                // otherwise keep previous value
                            }
                            else
                            {
                                d.maxLatency = result.Value;
                            }
                        }
                        // otherwise this result does not contribute.
                    }
                }
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
