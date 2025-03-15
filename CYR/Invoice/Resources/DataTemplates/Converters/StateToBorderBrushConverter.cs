using System.Globalization;
using CYR.Invoice.InvoiceModels;
using System.Windows.Data;
using System.Windows.Media;

namespace CYR.Invoice.Resources.DataTemplates.Converters
{
    public class StateToBorderBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (InvoiceState)value;
            return state == InvoiceState.Open ? new SolidColorBrush(Colors.Green) :
                                               new SolidColorBrush(Colors.Red);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
