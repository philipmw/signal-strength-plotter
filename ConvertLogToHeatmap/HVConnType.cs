using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertLogToHeatmap
{
    class HVConnType : IHeatValueConv
    {
        public int Value(Dictionary<string, string> point)
        {
            switch (point["netchoice"])
            {
                case "LTE":
                    return 40;
                case "EVDO_A":
                case "EVDO_Ae":
                case "EVDO_A_Extended":
                    return 30;
                case "EVDO_0":
                    return 20;
                case "RTT":
                case "RTT_Extended":
                    return 10;
                case "SearchingLTE":
                case "SearchingCDMA":
                case "SearchingGSM":
                case "AuthenticatingLTE":
                case "NoService":
                    return 1;
                default:
                    throw new Exception("Unrecognized Net Choice: " + point["netchoice"]);
            }
        }
    }
}
