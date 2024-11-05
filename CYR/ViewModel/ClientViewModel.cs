using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Clients;
using CYR.Services;
using System.Collections.ObjectModel;

namespace CYR.ViewModel
{
    public partial class ClientViewModel : ObservableObject, IParameterReceiver
    {
        private readonly IRetrieveClients _retrieveClients;
        public ClientViewModel(IRetrieveClients retrieveClients,INavigationService navigationService) 
        {
            _retrieveClients = retrieveClients;   
            Navigation = navigationService;
            Initialize();
        }

        private async void Initialize()
        {
            IEnumerable<Client> cl = await _retrieveClients.Handle();
            Clients = new ObservableCollection<Client>(cl);
        }
        [ObservableProperty]
        private INavigationService _navigation;
        [ObservableProperty]
        private ObservableCollection<Client>? _clients;

        [RelayCommand]
        public void CreateInvoice(Client client)
        {
            Navigation.NavigateTo<CreateInvoiceViewModel>(client);
        }
        [RelayCommand]
        private void CreateNewClient()
        {
            Navigation.NavigateTo<CreateClientViewModel>();
        }

        public void ReceiveParameter(object parameter)
        {            
            if (parameter != null) 
            {
                ObservableCollection<Client> cl = new ObservableCollection<Client>((List<Client>)parameter);
                if (cl != null)
                {
                    Clients = cl;                                    
                }
            }
        }
    }
}
