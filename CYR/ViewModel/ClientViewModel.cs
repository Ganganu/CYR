using CommunityToolkit.Mvvm.ComponentModel;
using CYR.Clients;
using CYR.Core;
using System.Collections.ObjectModel;

namespace CYR.ViewModel
{
    public partial class ClientViewModel : ObservableObject
    {
        private readonly IRetrieveClients _retrieveClients;
        public ClientViewModel(IRetrieveClients retrieveClients) 
        {
            this._retrieveClients = retrieveClients;
            Initialize();
        }

        private async void Initialize()
        {
            IEnumerable<Client> cl = (IEnumerable<Client>)await _retrieveClients.Handle();
            _clients = new ObservableCollection<Client>(cl);
        }
        [ObservableProperty]
        private ObservableCollection<Client>? _clients;
    }
}
