using System.Globalization;
using System.Windows.Data;

namespace CYR.Dashboard;

public class StatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int status)
        {
            return status == 1 ? "Bezahlt" : "Unbezahlt";
        }

        if (value is bool boolStatus)
        {
            return boolStatus ? "Bezahlt" : "Unbezahlt";
        }

        return "Unbezahlt"; // Default fallback
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
