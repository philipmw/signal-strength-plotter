using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Common.Latency
{
    [ValueConversion(typeof(long), typeof(Brush))]
    public class LatencyBackgroundConverter : IValueConverter
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
}
