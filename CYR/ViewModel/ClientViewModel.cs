using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Clients;
using CYR.Services;
using CYR.TestFolder;
using QuestPDF.Fluent;
using System.Collections.ObjectModel;
using System.Windows.Navigation;

namespace CYR.ViewModel
{
    public partial class ClientViewModel : ObservableObject
    {
        private readonly IRetrieveClients _retrieveClients;
        public ClientViewModel(IRetrieveClients retrieveClients,INavigationService navigationService) 
        {
            this._retrieveClients = retrieveClients;   
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
            Navigation.NavigateTo<InvoiceViewModel>(client);
            //Client client = (Client)parameter;
            //var model = InvoiceDocumentDataSource.GetInvoiceDetails(client);
            //var document = new InvoiceDocument(model);
            //document.GeneratePdfAndShow();
        }
        [RelayCommand]
        private void CreateNewClient()
        {
            Navigation.NavigateTo<CreateClientViewModel>();
        }
    }
}
