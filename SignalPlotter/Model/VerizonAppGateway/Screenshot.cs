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

        public enum NetworkChoice
        {
            LTE,
            EVDO_A,
            EVDO_Ae,
            EVDO_A_Extended,
            EVDO_0,
            RTT,
            RTT_Extended,
            SearchingLTE,
            SearchingCDMA,
            SearchingGSM,
            AuthenticatingLTE,
            NoService,
        };
        public enum ConnectedTo
        {
            FourG,
            ThreeG,
            TwoG,
        }
        public struct SignalDetail
        {
            public UInt16 bars2g, bars3g, bars4g; // 0--4
            public ConnectedTo? onWWAN;
        }
        public struct SignalStrengthSample
        {
            public string sumHash, detailHash;
            public NetworkChoice? netChoice;
            public SignalDetail? detail;
        }

        NetworkChoice? FindNetChoiceFromHash(string sumHash)
        {
            NetworkChoice? nc;
            switch (sumHash)
            {
                case "21c09df1c6beee7cf729d2a4e3631387ed9ca55e":
                    nc = NetworkChoice.LTE;
                    break;
                case "815c961636fdc0f45189aaffa22deaa462d8d956":
                    nc = NetworkChoice.EVDO_Ae;
                    break;
                case "315fa72f01ee893a19f3e8dbc6454f932ff77a4c":
                    nc = NetworkChoice.EVDO_A;
                    break;
                case "297be4bca7a5920ce2037b6f9232bb040330ea45":
                    nc = NetworkChoice.EVDO_A_Extended;
                    break;
                case "a5f6e7ea8a786a267d8a8108d2cfb6320e7d6b3f":
                    nc = NetworkChoice.EVDO_0;
                    break;
                case "4f48cc57922c371831b525fa52d91d8d8277451f":
                    nc = NetworkChoice.RTT;
                    break;
                case "def0ec3a8a0873b49cb1c99499b8106893038bb9":
                    nc = NetworkChoice.RTT_Extended;
                    break;
                case "0a913d78f5351fba57102274dd8bfc0d7b5a69ef":
                    nc = NetworkChoice.SearchingLTE;
                    break;
                case "d7a43c177ab95503c9218b662d924f2d635c2a08":
                    nc = NetworkChoice.SearchingCDMA;
                    break;
                case "56f1808cdf59059283403669299a5754ee9301ea":
                    nc = NetworkChoice.SearchingGSM;
                    break;
                case "4054126cbbaa506260f8b181f45880717bcae4f7":
                    nc = NetworkChoice.AuthenticatingLTE;
                    break;
                case "eeb0ea9b256bf72b6362667a00317698c17173df":
                    nc = NetworkChoice.NoService;
                    break;
                default:
                    Console.WriteLine("Unrecognized summary hash: " + sumHash);
                    //throw new UnrecognizedException();
                    nc = null;
                    break;
            }
            return nc;
        }

        SignalDetail? FindDetailFromHash(string detHash)
        {
            SignalDetail? sd;
            switch (detHash)
            {
                case "0e341c932fb0e6d3aa689153d0865c22b46853cd":
                    sd = new SignalDetail
                    {
                        bars4g = 4,
                        bars3g = 0,
                        bars2g = 0,
                        onWWAN = null,
                    };
                    break;
                case "ecd5c1da5f3ecb296e15fe9e8d705fe99cab8190":
                    sd = new SignalDetail
                    {
                        bars4g = 4,
                        bars3g = 0,
                        bars2g = 0,
                        onWWAN = ConnectedTo.FourG,
                    };
                    break;
                case "94677c479fadd409bfbbbccf16e2abc901309dc9":
                    sd = new SignalDetail
                    {
                        bars4g = 3,
                        bars3g = 0,
                        bars2g = 0,
                        onWWAN = null,
                    };
                    break;
                case "60f8830054a11575a56c5f27df6361e2d7f8c7de":
                    sd = new SignalDetail
                    {
                        bars4g = 3,
                        bars3g = 0,
                        bars2g = 0,
                        onWWAN = ConnectedTo.FourG,
                    };
                    break;
                case "fc03edc9f5f8736c81745288e9dd2706aeaeba10":
                    sd = new SignalDetail
                    {
                        bars4g = 2,
                        bars3g = 0,
                        bars2g = 0,
                        onWWAN = null,
                    };
                    break;
                case "c0ca8fe694473561972fb8fe2750e13b85f9ee1b":
                    sd = new SignalDetail
                    {
                        bars4g = 2,
                        bars3g = 0,
                        bars2g = 0,
                        onWWAN = ConnectedTo.FourG,
                    };
                    break;
                case "599e5e320abe91c141cb4a909acf899108476200":
                    sd = new SignalDetail
                    {
                        bars4g = 1,
                        bars3g = 0,
                        bars2g = 0,
                        onWWAN = null,
                    };
                    break;
                case "fc53e35f38340c3c687b9410f69bc71dc2984f98":
                    sd = new SignalDetail
                    {
                        bars4g = 1,
                        bars3g = 0,
                        bars2g = 0,
                        onWWAN = ConnectedTo.FourG,
                    };
                    break;
                case "65abb276336327d88a325e83636a289e3bf3bd58":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 4,
                        bars2g = 4,
                        onWWAN = null,
                    };
                    break;
                case "3aaf49316ed991df18ce3da61183717f803e3155":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 4,
                        bars2g = 4,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "e5ca6e959f5972ecdee10513d5d72afe4ee5ac22":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 4,
                        bars2g = 3,
                        onWWAN = null,
                    };
                    break;
                case "bd7d60dc0bc98f67c07869f2d8eeb5b7999f077a":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 4,
                        bars2g = 3,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "162cfd8bac5e1619e3aac7873517c6d599817a87":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 4,
                        bars2g = 2,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "87d89637fe068f3fc086a962d023923c7b4bb3ad":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 4,
                        bars2g = 1,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "e1ab307d11aa540aed5198d3a9a7ebd3e19ae0b8":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 4,
                        bars2g = 0,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "717aac63ac9a047746d69b38029d94538286819c":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 3,
                        bars2g = 4,
                        onWWAN = null,
                    };
                    break;
                case "8260b54cc32b1fbb625a3b8441d7f439023d7e0a":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 3,
                        bars2g = 4,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "40b1ff22be0f8c4383ad52bf6d138971cb1b3bd7":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 3,
                        bars2g = 3,
                        onWWAN = null,
                    };
                    break;
                case "138c245594b2a69039c86be51f762d78a5ef1626":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 3,
                        bars2g = 3,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "511193c6d5eda1513b3719e4f621da35ebd5379d":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 3,
                        bars2g = 2,
                        onWWAN = null,
                    };
                    break;
                case "afe3f861ab15d109aaffc2faaba3e14ea609bd5d":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 3,
                        bars2g = 2,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "5e18482031f7675b4ed86391edcb295d36cb773a":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 3,
                        bars2g = 1,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "c03d7ac29914c54c77bfdf6e86531f83af359c8a": // 2G available
                case "e92453eddb3ef1b950d8ac1f6ecc6e3342fd21c9": // 2G unavailable
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 3,
                        bars2g = 0,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "e6a0832c059880fee2e5ba8b0acee590d3f637aa":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 2,
                        bars2g = 4,
                        onWWAN = null,
                    };
                    break;
                case "cd6d2a4771ddd35d84bed12e03803650d52bf893":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 2,
                        bars2g = 4,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "14aff9c420a0d0857e413f4320c047fdee592778":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 2,
                        bars2g = 3,
                        onWWAN = null,
                    };
                    break;
                case "f9c1e77130a7fb254de199f132fbcd262f358606":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 2,
                        bars2g = 3,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "359721be673ff43646d5b8a7289355c281fce307":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 2,
                        bars2g = 2,
                        onWWAN = null,
                    };
                    break;
                case "4fe7f80e3ea61b175a675b49ede1534fd26acd4d":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 2,
                        bars2g = 2,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "1da03a54ec6bbf15226ed002f7798557771c0f24":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 2,
                        bars2g = 1,
                        onWWAN = null,
                    };
                    break;
                case "b3115616a1f4df70e0a65b5f2dd9fc05c3f38819":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 2,
                        bars2g = 1,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "795c6b0613640781b1b323e2d6a6157248c8b1a6":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 2,
                        bars2g = 0,
                        onWWAN = null,
                    };
                    break;
                case "ce54eedfff26f3fdf69fe62e04d47f4b6565d915":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 2,
                        bars2g = 0,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "1299a7f71dc937de378c6439d11302143b970549":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 1,
                        bars2g = 4,
                        onWWAN = null,
                    };
                    break;
                case "dc175cf69bf5c8cafc623d2703891d598490435d":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 1,
                        bars2g = 4,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "b2f7555a3e70d6b8469ea40cb3117fb2ef0b2bf9":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 1,
                        bars2g = 3,
                        onWWAN = null,
                    };
                    break;
                case "d34b2795192d8e827eb58e08e8d5016dc4eb0c65":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 1,
                        bars2g = 3,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "56a1755615d6814546938020f8fca2016a1665d0":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 1,
                        bars2g = 2,
                        onWWAN = null,
                    };
                    break;
                case "0e49850170c1dd3e2e4a7db9fd626b67f28319f0":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 1,
                        bars2g = 2,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "3dde33027c75cae68ca2c1c30bfa60aed7ebcf24":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 1,
                        bars2g = 1,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "cb610cbefe9daff6e2048ec08524e3c816b12dda":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 1,
                        bars2g = 1,
                        onWWAN = null,
                    };
                    break;
                case "493741b56da5413a31855340f1865f3ed39e44d3": // 2G and 3G are available
                case "61e8f0b1a111b589b9e60b86b0fe56ae616be60a": // only 3G is available
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 1,
                        bars2g = 0,
                        onWWAN = null,
                    };
                    break;
                case "d0d4ef863fddd5749178814fd324a1487adb10cc": // 2G is available
                case "81a6415add59ac453d7743b36e042afe09030d97": // 2G and 4G are unavailable
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 1,
                        bars2g = 0,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "b0c0491c330cada536eb96fc390f418e37fc575e":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 0,
                        bars2g = 4,
                        onWWAN = null,
                    };
                    break;
                case "aedeaecbeafdd5474caa87900f41bb9ac04d4586":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 0,
                        bars2g = 4,
                        onWWAN = ConnectedTo.TwoG,
                    };
                    break;
                case "f4e06745dafc5a2a0f2375e2f63c8b6b5cec3fe4": // only 2G is available
                case "ea3d13f5f3edb9647137ad0d440a2af0e92b4045": // 2G and 3G are available
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 0,
                        bars2g = 3,
                        onWWAN = null,
                    };
                    break;
                case "c8e09480f9cbc0e206d734411ddb5f1b0a5a3f31":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 0,
                        bars2g = 3,
                        onWWAN = ConnectedTo.TwoG,
                    };
                    break;
                case "c6ea0cbea5c8fce1abfde06b4ba360327475fc2c":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 0,
                        bars2g = 3,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "989e2a6049df70ac569f698e7aa2953e002891d2": // 3G is unavailable
                case "3e385dc151cd54c739f0df2cc97819d88c2e0a77": // 3G is available
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 0,
                        bars2g = 2,
                        onWWAN = null,
                    };
                    break;
                case "6975d83e5d7efbe03bfc032a517f7c67c42b0acf":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 0,
                        bars2g = 2,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "8575b0d7d746418670806bd6843d3c523cf65f5d":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 0,
                        bars2g = 2,
                        onWWAN = ConnectedTo.TwoG,
                    };
                    break;
                case "298aa403f7d5e0dd6c0bea29333d9c652002d66d": // 2G and 4G are available
                case "80fc494e9be12c7fcdfbbf02b069250bb5dd731c": // 2G and 3G are available
                case "415e00fe169f4458f43a1c9ad17605b0f002dab3": // only 2G is available
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 0,
                        bars2g = 1,
                        onWWAN = null,
                    };
                    break;
                case "c10796ba1a58b99d3430a7fdfd51f54fe2981e68":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 0,
                        bars2g = 1,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                case "87e0e53c0d58b18ed7f9feb13b5fda78d8d70c21":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 0,
                        bars2g = 1,
                        onWWAN = ConnectedTo.TwoG,
                    };
                    break;
                case "dc39b65ab4e89eb89794148ae06ca8f41cfcac4f": // only 3G is available
                case "c765aa7062d0ca4f17e391330beb3805fe2c2d3f": // only 4G is available
                case "a6059b33bd47ded4cb5d237330e7d3036dd79db5": // 3G and 2G are available
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 0,
                        bars2g = 0,
                        onWWAN = null,
                    };
                    break;
                case "16efc984c5a641db48e2abb7a15e31aabc2e38d2":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 0,
                        bars2g = 0,
                        onWWAN = ConnectedTo.FourG,
                    };
                    break;
                case "d709725776a4a5337995fff2a46f3df8fd97ca91": // 2G and 3G are available
                case "b753298634a99f08f2ee512226a510c2ad3215ef": // only 3G is available
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 0,
                        bars2g = 0,
                        onWWAN = ConnectedTo.ThreeG,
                    };
                    break;
                // "Unavailable"
                case "fd18816bb74206ef99dbc1fdb365a9993bcc1213":
                case "f30b6b62d1ff2c72a7b31615d04ee67c16ed45ee": // 2G
                case "72ed212b97d483f5155423d537748b28d2bcac0a":
                case "b94f046b4ea5ae08886cabba65791968a274d4a6":
                    sd = new SignalDetail
                    {
                        bars4g = 0,
                        bars3g = 0,
                        bars2g = 0,
                        onWWAN = null,
                    };
                    break;
                default:
                    Console.WriteLine("Unrecognized detail hash: " + detHash);
                    //throw new UnrecognizedException();
                    sd = null;
                    break;
            }
            return sd;
        }

        SignalStrengthSample analyzeSignalStrength(string sumHash, string detHash)
        {
            SignalStrengthSample sss = new SignalStrengthSample();

            sss.sumHash = sumHash;
            sss.detailHash = detHash;
            sss.netChoice = FindNetChoiceFromHash(sumHash);
            sss.detail = FindDetailFromHash(detHash);

            return sss;
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

            string sumHash, detHash;
            // We take a screenshot of the signal summary first.  Then we
            // activate the tooltip and take a screenshot of that too.
            try
            {
                sumHash = SnapSummary(d);
                detHash = SnapDetail(d);
            }
            catch (UnrecognizedException)
            {
                return null;
            }

            SignalStrengthSample sss = analyzeSignalStrength(sumHash, detHash);
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
