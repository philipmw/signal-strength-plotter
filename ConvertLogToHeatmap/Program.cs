using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertLogToHeatmap
{
    class Program
    {
        static Dictionary<string, string> CSVLineToDict(string[] csv)
        {
            Dictionary<string, string> d = new Dictionary<string,string>();
            d.Add("time", csv[0]);
            d.Add("sat-num", csv[1]);
            d.Add("lat-deg", csv[2]);
            d.Add("long-deg", csv[3]);
            d.Add("alt-m", csv[4]);
            d.Add("sumhash", csv[5]);
            d.Add("dethash", csv[6]);
            d.Add("netchoice", csv[7]);
            d.Add("bars4g", csv[8]);
            d.Add("bars3g", csv[9]);
            d.Add("bars2g", csv[10]);
            d.Add("onwwan", csv[11]);
            d.Add("latencylatest", csv[12]);
            d.Add("latencyema", csv[13]);
            return d;
        }

        static void ProcessData(StreamReader infile, StreamWriter outfile, IHeatValueConv hvc)
        {
            string line;
            StringBuilder sb = new StringBuilder();
            while ((line = infile.ReadLine()) != null)
            {
                line = infile.ReadLine();
                Debug.WriteLine(line);
                Dictionary<string, string> d = CSVLineToDict(line.Split(','));
                sb.Clear();
                sb.Append(d["lat-deg"] + "," + d["long-deg"] + "," + hvc.Value(d));
                outfile.WriteLine(sb);
            };
        }

        static void Main(string[] args)
        {
            try
            {
                using (StreamReader infile = new StreamReader("c:\\users\\pmw_000\\desktop\\VZ.txt"))
                {
                    using (StreamWriter outfile = new StreamWriter("c:\\users\\pmw_000\\desktop\\VZ-heat.csv"))
                    {
                        ProcessData(infile, outfile, new HVConnType());
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception: " + e.ToString());
            }
        }
    }
}
