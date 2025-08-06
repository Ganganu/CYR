using System.Windows.Media;
using CYR.Messages;

namespace CYR.Settings;

public interface ISelectImageService
{
    SnackbarMessage SelectImage();
    public string? SelectStringImage();
}