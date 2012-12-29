using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertLogToHeatmap
{
    interface IHeatValueConv
    {
        int Value(Dictionary<string, string> point);
    }
}
