using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SignalPlotter.Model.VerizonAppGateway
{
    class WindowSectionFinder
    {
        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        static IntPtr signalHwnd;

        static bool EnumWindowsCallback(IntPtr hwnd, IntPtr lParam)
        {
            int textLen = GetWindowTextLength(hwnd);
            StringBuilder text = new StringBuilder(textLen + 1);
            GetWindowText(hwnd, text, text.Capacity);
            if (text.ToString() == "Static")
            {
                signalHwnd = hwnd;
                return false;
            }
            return true;
        }

        public static IntPtr FindSignalSection(IntPtr vzWindow)
        {
            EnumChildWindows(vzWindow, new EnumWindowsProc(EnumWindowsCallback), new IntPtr(0));
            return signalHwnd;
        }
    }
}
