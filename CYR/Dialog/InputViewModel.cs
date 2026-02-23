using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace CYR.Dialog;

public partial class InputViewModel : ObservableRecipient
{
    [ObservableProperty]
    private string? _title;
    [ObservableProperty] 
    private string? _icon;
    [ObservableProperty]
    private string? _inputText;
    [ObservableProperty]
    private string? _inputTextHint;
    [ObservableProperty]
    private string? _okButtonText;
    [ObservableProperty]
    private string? _cancelButtonText;
    [ObservableProperty]
    private Visibility? _isOkVisible;
}
