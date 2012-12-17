using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SignalPlotter.Model
{
    class LatencyAnalyzer
    {
        Ping ping;
        IPAddress pingIp = new IPAddress(new byte[] { 4, 2, 2, 4 });

        public LatencyAnalyzer()
        {
            ping = new Ping();
        }

        long? PingOnce()
        {
            try
            {
                PingReply reply = ping.Send(pingIp, 2000);
                if (reply.Status != IPStatus.Success)
                    return null;
                else
                    return reply.RoundtripTime;
            }
            catch (PingException) // should happen only when exiting program
            {
                return null;
            }
        }

        // In my testing, the first ping is always much longer than subsequent
        // ones.  So let's ignore the highest ping value.  Then, to be fair,
        // let's also ignore the lowest ping value.
        public long? FindLatency()
        {
            short numPings = 4;
            long rttSum = 0, rttMin = long.MaxValue, rttMax = 0;
            for (short i = 0; i < numPings; ++i)
            {
                long? ms = PingOnce();
                if (ms.HasValue)
                {
                    rttSum += ms.Value;
                    rttMax = Math.Max(ms.Value, rttMax);
                    rttMin = Math.Min(ms.Value, rttMin);
                }
                else
                {
                    // even one timeout invalidates these results
                    return null;
                }
                System.Threading.Thread.Sleep((short)Math.Max(ms.Value, 1000));
            }
            rttSum -= (rttMax + rttMin);
            var pingAvg = rttSum / (numPings-2);
            return pingAvg;
        }
    }
}
