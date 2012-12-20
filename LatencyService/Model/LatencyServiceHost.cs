using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LatencyService.Model
{
    class LatencyServiceHost : ILatencyServiceContract
    {
        public LatestData LatestLatency()
        {
            PingService ps = ((App)Application.Current).pingService;
            ps.NotifyOfExternalRequest();
            return ps.LatestLatency();
        }

        public long Ema()
        {
            PingService ps = ((App)Application.Current).pingService;
            ps.NotifyOfExternalRequest();
            return ps.Ema();
        }
    }
}
