using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GpsService.ViewModel
{
    public class Cmds
    {
        private static RoutedUICommand requestGpsUpdate;

        static Cmds()
        {
            requestGpsUpdate = new RoutedUICommand("Request GPS Update", "Request GPS Update", typeof(Cmds));
        }

        public static RoutedUICommand RequestGpsUpdate
        {
            get { return requestGpsUpdate; }
        }
    }
}
