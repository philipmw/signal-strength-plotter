using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotSpatial.Positioning;
using System.ServiceModel;

namespace GpsSample
{
    class Program
    {
        static GpsService.GpsServiceContractClient gpsClient;

        public static void Main(string[] args)
        {
            gpsClient = new GpsService.GpsServiceContractClient();
            try
            {
                while (true)
                {
                    GpsService.LatestGpsData latest = gpsClient.GetLatest();
                    Console.WriteLine(string.Format("{0}: {1}  ({2} sat)", latest.time, latest.position, latest.satellites));
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }
            catch (CommunicationException e)
            {
                Console.WriteLine("Communication to GPS service failed: " + e.ToString());
            }
        }
    }
}
