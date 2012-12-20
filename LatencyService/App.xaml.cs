using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;

namespace LatencyService
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Model.PingService pingService;
        public ServiceHost serviceHost;

        public App()
        {
            pingService = new Model.PingService();
            serviceHost = new ServiceHost(typeof(Model.LatencyServiceHost));
            serviceHost.Open();
        }
    }
}
