using Microsoft.Win32;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CYR.Settings
{
    public class SelectImageService : ISelectImageService
    {
        public SelectImageService()
        {

        }

        public ImageSource SelectImage()
        {
            string path = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                path = openFileDialog.FileName;
            }
            ImageSource logo = PathToImageSource(path);
            return logo;
        }

        private ImageSource PathToImageSource(string path)
        {
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(path);
            logo.EndInit();
            return logo;
        }
    }
}
