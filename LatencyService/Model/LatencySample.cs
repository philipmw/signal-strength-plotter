using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatencyService.Model
{
    public enum SampleStatus { Nonexistent, TimedOut, Good };
    public struct LatencySample
    {
        public SampleStatus status;
        public long? rttMs; // value exists only if SampleStatus==Good
    }
}
