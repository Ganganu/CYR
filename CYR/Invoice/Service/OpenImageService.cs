using System.Diagnostics;
using System.Windows;

namespace CYR.Invoice.Service
{
    public class OpenImageService : IOpenImageService
    {
        public void OpenImage(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening image: {ex.Message}");
                }
            }
        }
    }
}
