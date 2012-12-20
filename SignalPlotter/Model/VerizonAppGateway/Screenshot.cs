using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SignalPlotter.Model.VerizonAppGateway
{
    class CursorPosition: IDisposable
    {
        int oldX, oldY;
        bool disposed;

        public CursorPosition(int x, int y)
        {
            disposed = false;
            oldX = System.Windows.Forms.Cursor.Position.X;
            oldY = System.Windows.Forms.Cursor.Position.Y;
            System.Windows.Forms.Cursor.Position = new Point(x, y);
        }

        ~CursorPosition()
        {
            if (!disposed)
                Dispose();
        }

        public void Dispose()
        {
            System.Windows.Forms.Cursor.Position = new Point(oldX, oldY);
            disposed = true;
        }
    }

    public class Screenshot
    {
        String bmpPath = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\Desktop\\VZ\\";
        const UInt16 tooltipMs = 1300; // time between cursor positioning and tooltip appearing or disappearing

        public class UnrecognizedException : Exception
        {
            public UnrecognizedException()
                : base("Did not recognize screenshot or section") { }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        IntPtr signalHwnd;
        public Screenshot(IntPtr signalHwnd_)
        {
            signalHwnd = signalHwnd_;
        }

        public enum NetworkChoice { SearchingLTE, LTE, EVDO_Ae, EVDO_0, RTT };
        public struct SignalStrength
        {
            public UInt16 bars2g, bars3g, bars4g; // 0--4
            public NetworkChoice netChoice;
        }
        public struct SignalStrengthSample
        {
            public string sumHash, detailHash;
            public SignalStrength? ss;
        }

        SignalStrength? analyzeSignalStrength(string sumHash, string detHash)
        {
            SignalStrength ss = new SignalStrength();
            if (sumHash == "21c09df1c6beee7cf729d2a4e3631387ed9ca55e")
                ss.netChoice = NetworkChoice.LTE;
            else if (sumHash == "815c961636fdc0f45189aaffa22deaa462d8d956")
                ss.netChoice = NetworkChoice.EVDO_Ae;
            else if (sumHash == "4f48cc57922c371831b525fa52d91d8d8277451f")
                ss.netChoice = NetworkChoice.RTT;
            else if (sumHash == "58dcd0b3b1b7d4e628e101d8339806cedb883bcf")
                ss.netChoice = NetworkChoice.SearchingLTE;
            else
            {
                Console.WriteLine("Unrecognized summary hash: " + sumHash);
                throw new UnrecognizedException();
            }

            // Remember: the first hash in a conditional is when I am not
            // connected.  The second is when I am.
            if (detHash == "0e341c932fb0e6d3aa689153d0865c22b46853cd" ||
                detHash == "ecd5c1da5f3ecb296e15fe9e8d705fe99cab8190")
            {
                ss.bars4g = 4;
                ss.bars3g = 0;
                ss.bars2g = 0;
            }
            else if (detHash == "94677c479fadd409bfbbbccf16e2abc901309dc9" ||
                detHash == "60f8830054a11575a56c5f27df6361e2d7f8c7de")
            {
                ss.bars4g = 3;
                ss.bars3g = 0;
                ss.bars2g = 0;
            }
            else if (detHash == "fc03edc9f5f8736c81745288e9dd2706aeaeba10" ||
                detHash == "c0ca8fe694473561972fb8fe2750e13b85f9ee1b")
            {
                ss.bars4g = 2;
                ss.bars3g = 0;
                ss.bars2g = 0;
            }
            else if (detHash == "599e5e320abe91c141cb4a909acf899108476200" ||
                detHash == "fc53e35f38340c3c687b9410f69bc71dc2984f98")
            {
                ss.bars4g = 1;
                ss.bars3g = 0;
                ss.bars2g = 0;
            }
            else if (detHash == "3aaf49316ed991df18ce3da61183717f803e3155")
            {
                ss.bars4g = 0;
                ss.bars3g = 4;
                ss.bars2g = 4;
            }
            else if (detHash == "bd7d60dc0bc98f67c07869f2d8eeb5b7999f077a")
            {
                ss.bars4g = 0;
                ss.bars3g = 4;
                ss.bars2g = 3;
            }
            else if (detHash == "162cfd8bac5e1619e3aac7873517c6d599817a87")
            {
                ss.bars4g = 0;
                ss.bars3g = 4;
                ss.bars2g = 2;
            }
            else if (detHash == "87d89637fe068f3fc086a962d023923c7b4bb3ad")
            {
                ss.bars4g = 0;
                ss.bars3g = 4;
                ss.bars2g = 1;
            }
            else if (detHash == "e1ab307d11aa540aed5198d3a9a7ebd3e19ae0b8")
            {
                ss.bars4g = 0;
                ss.bars3g = 4;
                ss.bars2g = 0;
            }
            else if (detHash == "8260b54cc32b1fbb625a3b8441d7f439023d7e0a")
            {
                ss.bars4g = 0;
                ss.bars3g = 3;
                ss.bars2g = 4;
            }
            else if (detHash == "40b1ff22be0f8c4383ad52bf6d138971cb1b3bd7" ||
                detHash == "138c245594b2a69039c86be51f762d78a5ef1626")
            {
                ss.bars4g = 0;
                ss.bars3g = 3;
                ss.bars2g = 3;
            }
            else if (detHash == "511193c6d5eda1513b3719e4f621da35ebd5379d" ||
                detHash == "afe3f861ab15d109aaffc2faaba3e14ea609bd5d")
            {
                ss.bars4g = 0;
                ss.bars3g = 3;
                ss.bars2g = 2;
            }
            else if (detHash == "c03d7ac29914c54c77bfdf6e86531f83af359c8a")
            {
                ss.bars4g = 0;
                ss.bars3g = 3;
                ss.bars2g = 0;
            }
            else if (detHash == "cd6d2a4771ddd35d84bed12e03803650d52bf893")
            {
                ss.bars4g = 0;
                ss.bars3g = 2;
                ss.bars2g = 4;
            }
            else if (detHash == "14aff9c420a0d0857e413f4320c047fdee592778" ||
                detHash == "f9c1e77130a7fb254de199f132fbcd262f358606")
            {
                ss.bars4g = 0;
                ss.bars3g = 2;
                ss.bars2g = 3;
            }
            else if (detHash == "359721be673ff43646d5b8a7289355c281fce307" ||
                detHash == "4fe7f80e3ea61b175a675b49ede1534fd26acd4d")
            {
                ss.bars4g = 0;
                ss.bars3g = 2;
                ss.bars2g = 2;
            }
            else if (detHash == "b3115616a1f4df70e0a65b5f2dd9fc05c3f38819")
            {
                ss.bars4g = 0;
                ss.bars3g = 2;
                ss.bars2g = 1;
            }
            else if (detHash == "795c6b0613640781b1b323e2d6a6157248c8b1a6" ||
                detHash == "ce54eedfff26f3fdf69fe62e04d47f4b6565d915")
            {
                ss.bars4g = 0;
                ss.bars3g = 2;
                ss.bars2g = 0;
            }
            else if (detHash == "d34b2795192d8e827eb58e08e8d5016dc4eb0c65")
            {
                ss.bars4g = 0;
                ss.bars3g = 1;
                ss.bars2g = 3;
            }
            else if (detHash == "0e49850170c1dd3e2e4a7db9fd626b67f28319f0")
            {
                ss.bars4g = 0;
                ss.bars3g = 1;
                ss.bars2g = 2;
            }
            else if (detHash == "d0d4ef863fddd5749178814fd324a1487adb10cc")
            {
                ss.bars4g = 0;
                ss.bars3g = 1;
                ss.bars2g = 0;
            }
            else if (detHash == "c8e09480f9cbc0e206d734411ddb5f1b0a5a3f31" ||
                detHash == "c6ea0cbea5c8fce1abfde06b4ba360327475fc2c")
            {
                ss.bars4g = 0;
                ss.bars3g = 0;
                ss.bars2g = 3;
            }
            else if (detHash == "8575b0d7d746418670806bd6843d3c523cf65f5d")
            {
                ss.bars4g = 0;
                ss.bars3g = 0;
                ss.bars2g = 2;
            }
            else if (detHash == "c765aa7062d0ca4f17e391330beb3805fe2c2d3f" ||
                detHash == "16efc984c5a641db48e2abb7a15e31aabc2e38d2" ||
                detHash == "d709725776a4a5337995fff2a46f3df8fd97ca91")
            {
                ss.bars4g = 0;
                ss.bars3g = 0;
                ss.bars2g = 0;
            }
            else
            {
                Console.WriteLine("Unrecognized detail hash: " + detHash);
                //throw new UnrecognizedException();
                return null;
            }
            return ss;
        }

        string SnapSummary(Dimensions d)
        {
            if (d.width != 554 || d.height != 36)
            {
                Console.WriteLine("Wrong dimensions (" + d.width + "x" + d.height + ").  Skipping.");
                throw new UnrecognizedException();
            }

            Bitmap bmp = new Bitmap(d.width-200, d.height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(d.rect.left, d.rect.top, 0, 0, bmp.Size);
            string summaryHash = hashBitmap(bmp);
            string destFilename = bmpPath + "sum " + summaryHash + ".bmp";
            if (!System.IO.File.Exists(destFilename))
                bmp.Save(destFilename, System.Drawing.Imaging.ImageFormat.Bmp);

            return summaryHash;
        }

        string SnapDetail(Dimensions d)
        {
            Bitmap bmp;
            using (CursorPosition curs = new CursorPosition(d.rect.right - 20, d.rect.top + 10))
            {
                System.Threading.Thread.Sleep(tooltipMs); // wait for tooltip to appear
                bmp = new Bitmap(95, 155, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(bmp);
                g.CopyFromScreen(d.rect.right + 10, d.rect.top - 60, 0, 0, bmp.Size);
            }

            string detailHash = hashBitmap(bmp);
            string destFilename = bmpPath + "det " + detailHash + ".bmp";
            if (!System.IO.File.Exists(destFilename))
                bmp.Save(destFilename, System.Drawing.Imaging.ImageFormat.Bmp);

            // Let's not wait for the tooltip to disappear because we're
            // already waiting long enough between samples.  This is redundant.
            //System.Threading.Thread.Sleep(tooltipMs); // wait for tooltip to disappear

            return detailHash;
        }

        public SignalStrengthSample? Snap()
        {
            SetForegroundWindow(signalHwnd);
            Dimensions d;
            try
            {
                d = WindowDimensions.GetDimensions(signalHwnd);
            }
            catch (WindowDimensions.DimensionsException)
            {
                return null;
            }

            SignalStrengthSample sss;
            // We take a screenshot of the signal summary first.  Then we
            // activate the tooltip and take a screenshot of that too.
            try
            {
                sss.sumHash = SnapSummary(d);
                sss.detailHash = SnapDetail(d);
            }
            catch (UnrecognizedException)
            {
                return null;
            }

            sss.ss = analyzeSignalStrength(sss.sumHash, sss.detailHash);
            return sss;
        }

        String hashBitmap(Bitmap bmp)
        {
            // http://msdn.microsoft.com/en-us/library/5ey6h79d.aspx
            Rectangle r = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpd = bmp.LockBits(r, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            byte[] rgb = new byte[Math.Abs(bmpd.Stride) * bmp.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpd.Scan0, rgb, 0, rgb.Length);
            bmp.UnlockBits(bmpd);

            // http://msdn.microsoft.com/en-us/library/s02tk69a.aspx
            byte[] hash = new System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(rgb);
            StringBuilder s = new StringBuilder();
            foreach (byte b in hash)
            {
                s.Append(b.ToString("x2"));
            }
            return s.ToString();
        }
    }
}
