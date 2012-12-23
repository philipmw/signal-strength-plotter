using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using SignalPlotter.Model.VerizonAppGateway;

namespace SignalPlotter.Model
{
    public struct Sample
    {
        public Screenshot.SignalStrengthSample? sss;
        public PmwGpsService.LatestGpsData gps;
        public PmwLatencyService.LatestData latency;
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

        ulong samplesCollected;

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

        public event EventHandler<Sample?> SampleAvailable;

        Sample? GetSample()
        {
            Sample s;
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
                MessageBox.Show(DateTime.Now + ": Unable to communicate with GPS Service!  Not logging any data until this is fixed!");
                gpsClient = null;
                return null;
            }

            // FIXME: We need to track previous position and calculate distance,
            // rather than looking at speed.
            if (samplesCollected > 0 && s.gps.speed5sec < DotSpatial.Positioning.Speed.FromFeetPerSecond(7))
            {
                Console.WriteLine("Skipping sample collection because our speed is only " + s.gps.speed5sec.ToImperialUnitType());
                return null;
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
                MessageBox.Show(DateTime.Now + ": Unable to communicate with Latency Service!  Not logging any data until this is fixed!");
                latencyClient = null;
                return null;
            }

            s.sss = vzSignalScreenshotter.Snap();

            ++samplesCollected;
            return s;
        }

        void ThreadRun()
        {
            while (true)
            {
                try
                {
                    Sample? s = GetSample();
                    if (SampleAvailable != null)
                        SampleAvailable(this, s);
                }
                catch (CommunicationException)
                {
                    // We already displayed an error message and handled the situation.
                }
                System.Threading.Thread.Sleep(5000);
            }
        }
    }
}
