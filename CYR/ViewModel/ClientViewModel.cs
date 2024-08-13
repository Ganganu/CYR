using CommunityToolkit.Mvvm.ComponentModel;
using CYR.Clients;
using CYR.Core;
using System.Collections.ObjectModel;

namespace CYR.ViewModel
{
    public partial class ClientViewModel : ObservableObject
    {
        public ClientViewModel() 
        {

        }

        [ObservableProperty]
        private ObservableCollection<Client> _clients;
    }
}
