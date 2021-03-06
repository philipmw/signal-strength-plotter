﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SignalPlotter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Model.Logger logger = new Model.Logger();

        public App()
        {
            Model.MainThread.Instance.SampleAvailable += logger.SampleAvailable;
        }
    }
}
