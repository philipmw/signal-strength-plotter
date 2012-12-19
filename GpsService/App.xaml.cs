using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;

namespace GpsService
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Model.GpsThread gpsThread;
        public ServiceHost serviceHost;

        App()
        {
            serviceHost = new ServiceHost(typeof(Model.GpsServiceHost));
            serviceHost.Open();
        }
    }
}
