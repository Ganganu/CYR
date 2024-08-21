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
        public void CreateInvoice()
        {
            var model = InvoiceDocumentDataSource.GetInvoiceDetails();
            var document = new InvoiceDocument(model, Clients[0]);
            document.GeneratePdfAndShow();
        }
    }
}
