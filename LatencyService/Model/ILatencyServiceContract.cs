using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LatencyService.Model
{
    public struct LatestData
    {
        public LatencySample latest;
        public LatencySample ema;
    }

    [ServiceContract]
    interface ILatencyServiceContract
    {
        [OperationContract]
        LatestData LatestLatency();
    }
}
