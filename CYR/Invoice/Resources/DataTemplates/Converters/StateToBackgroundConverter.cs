using System.Globalization;
using CYR.Invoice.InvoiceModels;
using System.Windows.Data;
using System.Windows.Media;

namespace CYR.Invoice.Resources.DataTemplates.Converters
{
    public class StateToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (InvoiceState)value;
            return state == InvoiceState.Open ? new SolidColorBrush(Color.FromRgb(235, 249, 235)) :
                                               new SolidColorBrush(Color.FromRgb(253, 235, 235));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
