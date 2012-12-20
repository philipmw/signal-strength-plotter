using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpsService.Model
{
    class TimeBasedCounter
    {
        ConcurrentQueue<DateTime> log = new ConcurrentQueue<DateTime>();
        TimeSpan tspan;

        public TimeBasedCounter(TimeSpan t)
        {
            tspan = t;
        }

        public void Add()
        {
            log.Enqueue(DateTime.Now);
        }

        public bool Recount()
        {
            bool deletedSome = false;
            while (log.Count > 0 && (DateTime.Now - log.First()) > tspan)
            {
                deletedSome = true;
                DateTime expiredStamp;
                log.TryDequeue(out expiredStamp);
            }
            return deletedSome;
        }

        public int Count()
        {
            Recount();
            return log.Count();
        }
    }
}
