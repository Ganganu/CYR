using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using CYR.Invoice.InvoiceModels;

namespace CYR.Invoice.Resources.DataTemplates.Converters
{
    public class StateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is InvoiceState state)
            {
                var field = state.GetType().GetField(state.ToString());
                var displayAttribute = field?.GetCustomAttribute<DisplayAttribute>();
                return displayAttribute?.GetName() ?? state.ToString();
            }
            return value.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
