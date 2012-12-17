using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalPlotter.Model.VerizonAppGateway
{
    public class ProcessException : Exception
    {
        public ProcessException()
            : base("Could not find VZAccess Manager") { }
    }

    class FindProcess
    {
        public static IntPtr getVZWindow()
        {
            Process[] vzProcs = Process.GetProcessesByName("VZAccess Manager");
            if (vzProcs.Length != 1 || vzProcs[0].MainWindowHandle.ToInt32() == 0)
                throw new ProcessException();
            else
                return vzProcs[0].MainWindowHandle;
        }
    }
}
