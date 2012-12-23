using SignalPlotter.PmwLatencyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SignalPlotter.View
{
    [ValueConversion(typeof(long), typeof(Brush))]
    class LatencyBackgroundConverter : IValueConverter
    {
        public static Brush TimeToColor(long t)
        {
            Brush b;
            if (t < 70)
                b = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
            else if (t < 110)
                b = new SolidColorBrush(Color.FromArgb(255, 170, 255, 0));
            else if (t < 150)
                b = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
            else if (t < 200)
                b = new SolidColorBrush(Color.FromArgb(255, 255, 127, 0));
            else
                b = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            return b;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return TimeToColor((long)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(LatencySample), typeof(string))]
    class SampleValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            LatencySample sample = (LatencySample)value;
            string v;
            switch (sample.status)
            {
                case SampleStatus.Good:
                    v = sample.rttMs.ToString();
                    break;
                case SampleStatus.Nonexistent:
                    v = "--";
                    break;
                case SampleStatus.TimedOut:
                    v = "##";
                    break;
                default:
                    v = "err";
                    break;
            }
            return v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(LatencySample), typeof(Brush))]
    class SampleBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Brush bgBrush;
            LatencySample sample = (LatencySample)value;
            switch (sample.status)
            {
                case SampleStatus.Good:
                    bgBrush = LatencyBackgroundConverter.TimeToColor(sample.rttMs.Value);
                    break;
                case SampleStatus.TimedOut:
                    bgBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 255));
                    break;
                case SampleStatus.Nonexistent:
                    bgBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    break;
                default:
                    bgBrush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
                    break;
            }
            return bgBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
