﻿using System;
using System.Collections.Generic;
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

        public void SampleAvailable(object sender, Model.Sample s)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(s.gps.time.ToString("s") + ",");
            sb.Append(s.gps.satellites + ",");
            sb.Append(s.gps.position.Latitude.DecimalDegrees.ToString("F9") + ",");
            sb.Append(s.gps.position.Longitude.DecimalDegrees.ToString("F9") + ",");
            sb.Append(s.gps.position.Altitude.ToMeters().Value.ToString("F2") + ",");

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

            sb.Append(s.latency.ema.ToString() + ",");
            sb.Append(s.latency.minLatency.ToString() + ",");
            sb.Append(s.latency.maxLatency.ToString());

            dataStream.WriteLine(sb);
            dataStream.FlushAsync();
        }

        public void Dispose()
        {
            dataStream.Dispose();
        }
    }
}