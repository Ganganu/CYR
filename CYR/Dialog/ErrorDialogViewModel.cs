using CommunityToolkit.Mvvm.ComponentModel;

namespace CYR.Dialog
{
    public partial class ErrorDialogViewModel : ObservableObject
    {
        public ErrorDialogViewModel()
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
        private string? _iconUrl;
        [ObservableProperty]
        private string? _okButtonText;
        [ObservableProperty]
        private string? _cancelButtonText;
    }
}
