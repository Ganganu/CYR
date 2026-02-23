using CYR.Messages;
using Microsoft.Win32;
using System.Windows.Media.Imaging;

namespace CYR.Settings;

public class SelectImageService : ISelectImageService
{
    public string? SelectStringImage()
    {
        string path = string.Empty;
        OpenFileDialog openFileDialog = new();
        if (openFileDialog.ShowDialog() == true)
        {
            path = openFileDialog.FileName;
        }
        string logo = path;
        return logo;
    }

    public SnackbarMessage SelectImage()
    {
        string path = string.Empty;
        OpenFileDialog openFileDialog = new();
        if (openFileDialog.ShowDialog() == true)
        {
            path = openFileDialog.FileName;
        }
        SnackbarMessage logo = PathToImageSource(path);
        return logo;
    }

    private static SnackbarMessage PathToImageSource(string path)
    {
        BitmapImage logo = new();
        try
        {            
            logo.BeginInit();
            logo.UriSource = new Uri(path);
            logo.EndInit();            
        }
        catch (Exception)
        {
            return new SnackbarMessage("Fehler. Versuchen sie es erneut ein Bild auszuwählen!", "Warning");
        }
        return new SnackbarMessage("Logo erfolgreich aktualisiert.","Check", logo,ImagePath: path);
    }
}
