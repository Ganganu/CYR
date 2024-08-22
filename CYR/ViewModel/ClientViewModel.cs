using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Clients;
using CYR.TestFolder;
using QuestPDF.Fluent;
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

        [RelayCommand]
        public void CreateInvoice(object parameter)
        {
            Client client = (Client)parameter;
            var model = InvoiceDocumentDataSource.GetInvoiceDetails(client);
            var document = new InvoiceDocument(model);
            document.GeneratePdfAndShow();
        }
    }
}
