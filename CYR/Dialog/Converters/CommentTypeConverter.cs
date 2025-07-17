using System.Globalization;
using System.Windows.Data;

namespace CYR.Dialog.Converters;

public class CommentTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CommentType location)
        {            
            return location switch
            {
                CommentType.Top => "Kommentaren Oben",
                CommentType.Bottom => "Kommentaren Unten",
                _ => value.ToString()
            };
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
