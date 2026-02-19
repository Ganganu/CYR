using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CYR.Dialog;

public partial class NotificationViewModel : ObservableRecipient
{
    public NotificationViewModel()
    {
          
    }
    [ObservableProperty]
    private string? _title;
    [ObservableProperty]
    private string? _message;
    [ObservableProperty]
    private string? _description;
    [ObservableProperty]
    private string? _icon;
    [ObservableProperty]
    private string? _okButtonText;
    [ObservableProperty]
    private string? _cancelButtonText;
    [ObservableProperty]
    private Visibility? _isOkVisible;
    [ObservableProperty]
    private BitmapImage? _image;
}
