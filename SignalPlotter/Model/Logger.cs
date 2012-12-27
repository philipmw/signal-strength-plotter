using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalPlotter.Model
{
    class Logger : IDisposable
    {
        StreamWriter dataStream;

        public Logger()
        {
            string outputFilename = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\Desktop\\vz-output.txt";
            dataStream = File.AppendText(outputFilename);
            dataStream.WriteLine("Opened for appending at " + DateTime.Now);
        }

        public void SampleAvailable(object sender, Sample s)
        {
            if (s.ssss == SignalStrengthSampleStatus.TooProximateToPrev)
            {
                Debug.WriteLine("Not writing entry to log because it's too proximate to previous sample.");
                return;
            }
            StringBuilder sb = new StringBuilder();

            WriteSampleData(ref sb, s);

            dataStream.WriteLine(sb);
            dataStream.FlushAsync();
        }


        void WriteSampleData(ref StringBuilder sb, Sample s)
        {
            if (!s.gps.HasValue)
            {
                sb.Append("No GPS data");
                return;
            }
            else
            {
                PmwGpsService.LatestGpsData gps = s.gps.Value;
                sb.Append(gps.time.ToString("s") + ",");
                sb.Append(gps.satellites + ",");
                sb.Append(gps.position.Latitude.DecimalDegrees.ToString("F8") + ",");
                sb.Append(gps.position.Longitude.DecimalDegrees.ToString("F8") + ",");
                sb.Append(gps.position.Altitude.ToMeters().Value.ToString("F1") + ",");
            }

            if (s.sss.HasValue)
            {
                VerizonAppGateway.Screenshot.SignalStrengthSample sss = s.sss.Value;
                sb.Append(sss.sumHash + ",");
                sb.Append(sss.detailHash + ",");
                if (sss.ss.HasValue)
                {
                    VerizonAppGateway.Screenshot.SignalStrength ss = sss.ss.Value;
                    sb.Append(ss.netChoice.ToString("g") + ",");
                    sb.Append(ss.bars4g + ",");
                    sb.Append(ss.bars3g + ",");
                    sb.Append(ss.bars2g + ",");
                }
                else
                {
                    sb.Append("NET-UNK,4G-UNK,3G-UNK,2G-UNK,");
                }
            }
            else
            {
                sb.Append("NO-HASH,NO-HASH,NET-UNK,4G-UNK,3G-UNK,2G-UNK,");
            }

            if (s.latency.HasValue)
            {
                PmwLatencyService.LatestData lat = s.latency.Value;
                sb.Append(LatencySampleToCSV(lat.latest) + ",");
                sb.Append(LatencySampleToCSV(lat.ema) + ",");
            }
            else
            {
                sb.Append("NO-PING,NO-EMA,");
            }
        }

        public void Dispose()
        {
            dataStream.Dispose();
        }

        string LatencySampleToCSV(PmwLatencyService.LatencySample ls)
        {
            string result;
            switch (ls.status)
            {
                case PmwLatencyService.SampleStatus.Good:
                    result = ls.rttMs.ToString();
                    break;
                case PmwLatencyService.SampleStatus.Nonexistent:
                    result = "N/A";
                    break;
                case PmwLatencyService.SampleStatus.TimedOut:
                    result = "TimedOut";
                    break;
                default:
                    result = "Error";
                    break;
            }
            return result;
        }
    }
}
