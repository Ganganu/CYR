using CommunityToolkit.Mvvm.ComponentModel;

namespace CYR.Dialog
{
    public partial class ItemsListDialogViewModel : ObservableRecipient
    {
        public ItemsListDialogViewModel()
        {
            
        }

        [ObservableProperty]
        private string _title;
        [ObservableProperty]
        private List<string> _files;
    }
}
