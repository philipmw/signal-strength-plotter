using SignalPlotter.Model.VerizonAppGateway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SignalPlotter.ViewModel
{
    [ValueConversion(typeof(Screenshot.ConnectedTo?), typeof(Brush))]
    public class ConnectedToConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Screenshot.ConnectedTo? v = (Screenshot.ConnectedTo?)value;
            if (v.HasValue)
            {
                switch (v.Value)
                {
                    case Screenshot.ConnectedTo.FourG:
                        return new SolidColorBrush(Color.FromArgb(255, 128, 128, 255));
                    case Screenshot.ConnectedTo.ThreeG:
                        return new SolidColorBrush(Color.FromArgb(255, 128, 255, 128));
                    case Screenshot.ConnectedTo.TwoG:
                        return new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
                    default:
                        throw new Exception("Converter does not recognize ConnectedTo value!");
                }
            }
            else
            {
                return new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
