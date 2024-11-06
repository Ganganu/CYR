using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Invoice;
using System.Collections.ObjectModel;

namespace CYR.ViewModel
{
    public partial class GetInvoiceViewModel : ObservableObject
    {
        private readonly IInvoiceRepository _invoiceRepository;
        public GetInvoiceViewModel(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
            Initialize();
        }
        private async void Initialize()
        {
            IEnumerable<InvoiceModel> invoices = await _invoiceRepository.GetAllAsync(); 
            Invoices = new ObservableCollection<InvoiceModel>(invoices);
        }

        [ObservableProperty]
        private ObservableCollection<InvoiceModel>? _invoices;
        [RelayCommand]
        private void SelectInvoice(object parameter)
        {

        }
    }
}
