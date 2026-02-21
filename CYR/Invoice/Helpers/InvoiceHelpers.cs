using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CYR.Invoice.Helpers;

public static class InvoiceHelpers
{
    public static ImageSource? ConvertStringToLogo(string? logoPath)
    {
        if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(logoPath, UriKind.Absolute);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                return image;
            }
            catch (Exception)
            {
                throw;
            }
        }
        return null;
    }
}
