using Common;
using DotSpatial.Positioning;
using SignalPlotter.Model.VerizonAppGateway;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SignalPlotter.Model
{
    public enum SignalStrengthSampleStatus { Normal, TooProximateToPrev };
    public struct Sample
    {
        public SignalStrengthSampleStatus ssss;
        public Screenshot.SignalStrengthSample? sss;
        public PmwGpsService.LatestGpsData? gps;
        public PmwLatencyService.LatestData? latency;
    }

    class MainThread : IDisposable
    {
        static MainThread inst;
        static Object syncroot = new Object();

        // http://www.blackwasp.co.uk/DisableScreensaver.aspx
        [FlagsAttribute]
        enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
        }
        [DllImport("kernel32.dll", CharSet = CharSet.Auto,SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        bool disposed = false;

        MainThread()
        {
        }

        public void Dispose()
        {
            if (!disposed)
            {
                Stop(this, new CancelEventArgs());
                disposed = true;
            }
        }

        public static MainThread Instance
        {
            get
            {
                if (inst == null)
                {
                    lock (syncroot)
                    {
                        if (inst == null)
                            inst = new MainThread();
                    }
                }
                return inst;
            }
        }

        IntPtr vzWindow;
        IntPtr signalHandle;
        Screenshot vzSignalScreenshotter;

        PmwGpsService.GpsServiceContractClient gpsClient;
        PmwLatencyService.LatencyServiceContractClient latencyClient;

        Position3D? prevPosition;

        Thread t;
        public void Start(object sender, RoutedEventArgs e)
        {
            try
            {
                vzWindow = FindProcess.getVZWindow();
            }
            catch (ProcessException ex)
            {
                MessageBox.Show("Cannot start: " + ex.Message);
                Application.Current.Shutdown(1);
            }
            signalHandle = WindowSectionFinder.FindSignalSection(vzWindow);
            vzSignalScreenshotter = new Screenshot(signalHandle);

            t = new Thread(ThreadRun);
            SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED |
                EXECUTION_STATE.ES_SYSTEM_REQUIRED |
                EXECUTION_STATE.ES_CONTINUOUS);
            t.Start();
        }

        public void Stop(object sender, CancelEventArgs e)
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS); // resume screensaver
            if (t != null)
            {
                t.Abort();
                t.Join();
            }
        }

        public event EventHandler<Sample> SampleAvailable;

        Distance MinDistanceBetweenSamples = new Distance(0.1, DistanceUnit.StatuteMiles);
        Sample GetSample()
        {
            Sample s = new Sample();
            try
            {
                if (gpsClient == null)
                {
                    gpsClient = new PmwGpsService.GpsServiceContractClient();
                }
                s.gps = gpsClient.GetLatest();
            }
            catch (CommunicationException)
            {
                Debug.WriteLine("Unable to communicate with GPS Service!");
                gpsClient = null;
                s.gps = null;
            }

            try
            {
                if (latencyClient == null)
                {
                    latencyClient = new PmwLatencyService.LatencyServiceContractClient();
                }
                s.latency = latencyClient.LatestLatency();
            }
            catch (CommunicationException)
            {
                Debug.WriteLine("Unable to communicate with Latency Service!");
                latencyClient = null;
                s.latency = null;
            }

            // Determine if we care about the signal strength now.
            if (s.gps.HasValue &&
                prevPosition.HasValue &&
                s.gps.Value.position.DistanceFromOnEarth(prevPosition.Value) < MinDistanceBetweenSamples)
            {
                Debug.WriteLine("Skipping sample collection because the previous sample is too proximate.");
                s.ssss = SignalStrengthSampleStatus.TooProximateToPrev;
                s.sss = null;
            }
            else
            {
                s.ssss = SignalStrengthSampleStatus.Normal;
                s.sss = vzSignalScreenshotter.Snap();
            }

            if (s.gps.HasValue)
            {
                prevPosition = s.gps.Value.position;
            }
            return s;
        }

        void ThreadRun()
        {
            while (true)
            {
                try
                {
                    Sample s = GetSample();
                    SampleAvailable(this, s);
                }
                catch (CommunicationException)
                {
                    // We already displayed an error message and handled the situation.
                }
                System.Threading.Thread.Sleep(8000);
            }
        }
    }
}
