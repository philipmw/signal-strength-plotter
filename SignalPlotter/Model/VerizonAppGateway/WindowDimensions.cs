using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SignalPlotter.Model.VerizonAppGateway
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left, top, right, bottom;
    }

    public class WindowDimensions
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        public class DimensionsException : Exception
        {
            public DimensionsException() : base("Window dimensions are invalid") { }
        }
        public static Dimensions GetDimensions(IntPtr hwnd)
        {
            RECT r;
            GetWindowRect(hwnd, out r);
            Dimensions d = new Dimensions { rect = r, height = (r.bottom - r.top), width = (r.right - r.left) };

            if (d.width == 0 || d.height == 0)
                throw new DimensionsException();
            return d;
        }
    }

    public struct Dimensions
    {
        public int height, width;
        public RECT rect;
    };
}
