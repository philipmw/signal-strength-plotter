using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using SignalPlotter.Model.VerizonAppGateway;

namespace SignalPlotter.Model
{
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
        bool screensaverStopped = false;
        LatencyAnalyzer latencyAnalyzer;

        MainThread()
        {
            latencyAnalyzer = new LatencyAnalyzer();
        }

        void resumeScreensaver()
        {
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
            screensaverStopped = false;
        }

        public void Dispose()
        {
            if (!disposed)
            {
                resumeScreensaver();
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

        Thread t;
        public void start(object sender, RoutedEventArgs e)
        {
            t = new Thread(run);
            SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED |
                EXECUTION_STATE.ES_SYSTEM_REQUIRED |
                EXECUTION_STATE.ES_CONTINUOUS);
            screensaverStopped = true;
            t.Start();
        }

        public void stop(object sender, CancelEventArgs e)
        {
            resumeScreensaver();
            t.Abort();
            t.Join();
        }

        public delegate void SignalStrengthDelegate(Screenshot.SignalStrength? ss);
        public event SignalStrengthDelegate SignalStrengthAvailable;

        public delegate void GpsInfoDelegate(string timestamp);
        public event GpsInfoDelegate GpsInfoAvailable;

        public delegate void LatencyDelegate(long? latency);
        public event LatencyDelegate LatencyAvailable;

        void run()
        {
            IntPtr vzWindow = FindProcess.getVZWindow();
            IntPtr signalHandle = WindowSectionFinder.FindSignalSection(vzWindow);
            Screenshot vzSignalScreenshotter = new Screenshot(signalHandle);
            while (true)
            {
                Screenshot.SignalStrength? ss = vzSignalScreenshotter.Snap();
                if (SignalStrengthAvailable != null)
                {
                    SignalStrengthAvailable(ss);
                }

                // FIXME: read GPS here
                if (GpsInfoAvailable != null)
                {
                    GpsInfoAvailable(DateTime.Now.ToString());
                }

                if (LatencyAvailable != null)
                {
                    LatencyAvailable(latencyAnalyzer.FindLatency());
                }

                System.Threading.Thread.Sleep(5000);
            }
        }
    }
}
