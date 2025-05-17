using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CYR.Dialog
{
    public partial class ItemsListDialogViewModel : ObservableRecipient
    {
        public ItemsListDialogViewModel()
        {

        }

        [ObservableProperty]
        private string? _title;
        [ObservableProperty]
        private ObservableCollection<string>? _files;
        [ObservableProperty]
        private string? _icon;
    }
}
