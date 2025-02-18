using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CYR.Invoice;
using CYR.Services;
using System.Collections.ObjectModel;

namespace CYR.ViewModel
{
    public partial class GetInvoiceViewModel : ObservableObject
    {
        private readonly IInvoiceRepository _invoiceRepository;
        public GetInvoiceViewModel(IInvoiceRepository invoiceRepository, INavigationService navigationService)
        {
            _invoiceRepository = invoiceRepository;
            NavigationService = navigationService;
            Initialize();
        }
        private async void Initialize()
        {
            IEnumerable<InvoiceModel> invoices = await _invoiceRepository.GetAllAsync(); 
            Invoices = new ObservableCollection<InvoiceModel>(invoices);
        }

        [ObservableProperty]
        private ObservableCollection<InvoiceModel>? _invoices;

        public INavigationService NavigationService { get; }

        [RelayCommand]
        private void SelectInvoice(object parameter)
        {
            InvoiceModel invoice = (InvoiceModel)parameter;
            NavigationService.NavigateTo<ShowInvoiceViewModel>(invoice.InvoiceNumber);
        }
    }
}
