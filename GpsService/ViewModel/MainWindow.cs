﻿using DotSpatial.Positioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GpsService.ViewModel
{
    class MainWindow : BaseViewModel
    {
        string gpsDevice;
        public string GpsDevice
        {
            get { return gpsDevice; }
            private set
            {
                gpsDevice = value;
                NotifyPropertyChanged();
            }
        }

        string gpsTime;
        public string GpsTime
        {
            get { return gpsTime; }
            private set
            {
                gpsTime = value;
                NotifyPropertyChanged();
            }
        }

        string gpsLong;
        public string GpsLong
        {
            get { return gpsLong; }
            private set
            {
                gpsLong = value;
                NotifyPropertyChanged();
            }
        }

        string gpsLat;
        public string GpsLat
        {
            get { return gpsLat; }
            private set
            {
                gpsLat = value;
                NotifyPropertyChanged();
            }
        }

        string altitude;
        public string Altitude
        {
            get { return altitude; }
            private set
            {
                altitude = value;
                NotifyPropertyChanged();
            }
        }
        ushort satellites;
        public ushort Satellites
        {
            get { return satellites; }
            private set
            {
                satellites = value;
                NotifyPropertyChanged();
            }
        }

        ulong extReqsIn1Min;
        public ulong ExtReqsIn1Min
        {
            get { return extReqsIn1Min; }
            private set
            {
                extReqsIn1Min = value;
                NotifyPropertyChanged();
            }
        }

        public MainWindow()
        {
            GpsDevice = "None";
        }

        public void Start()
        {
            Model.GpsThread thr;
            thr = ((App)Application.Current).gpsThread = new Model.GpsThread();
            thr.RequestProcessed += RequestProcessed;
            thr.GpsDataUpdated += Update;
        }

        public void Close()
        {
            ((App)Application.Current).gpsThread.Dispose();
            ((App)Application.Current).gpsThread = null;
        }

        public void Update()
        {
            Model.GpsThread t = ((App)Application.Current).gpsThread;
            GpsDevice = t.DevName;
            GpsLong = t.GpsPos.Longitude.ToString();
            GpsLat = t.GpsPos.Latitude.ToString();
            Altitude = t.Altitude.ToFeet().ToString();
            Satellites = t.Satellites;
            GpsTime = t.GpsTime.ToString();
        }

        void RequestProcessed(object sender, Model.GpsThread.RequestProcessedEventArgs e)
        {
            ExtReqsIn1Min = e.requestsInLastMinute;
        }
    }
}
