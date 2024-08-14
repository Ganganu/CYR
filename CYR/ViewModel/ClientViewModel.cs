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
            IEnumerable<Client> cl = await _retrieveClients.Handle();
            Clients = new ObservableCollection<Client>(cl);
        }
        [ObservableProperty]
        private ObservableCollection<Client>? _clients;
    }
}
