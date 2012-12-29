using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertLogToHeatmap
{
    class HVLatency : IHeatValueConv
    {
        int timeOutValue;

        public HVLatency(int timeOutValue_ = 2000)
        {
            timeOutValue = timeOutValue_;
        }

        public int Value(Dictionary<string, string> point)
        {
            string latency = point["latencylatest"];
            if (latency == "TimedOut")
                return timeOutValue;
            else
                return UInt16.Parse(latency);
        }
    }
}
