using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Invoice.InvoiceModels;
using CYR.Invoice.InvoiceRepositorys;
using CYR.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace CYR.Invoice.InvoiceViewModels
{
    public partial class InvoiceListViewModel : ObservableRecipient
    {
        private readonly IInvoiceRepository _invoiceRepository;
        

        public InvoiceListViewModel(IInvoiceRepository invoiceRepository,
            INavigationService navigationService)
        {
            _invoiceRepository = invoiceRepository;
            NavigationService = navigationService;
            Initialize();
        }
        private async void Initialize()
        {
            IEnumerable<InvoiceModel> invoices = await _invoiceRepository.GetAllAsync();
            Invoices = [.. invoices];
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
        [RelayCommand]
        private void SetInvoiceStateClosed()
        {
            foreach (var item in Invoices)
            {
                if (item.State == InvoiceState.Open && (bool)item.IsSelected)
                {                    
                    item.State = InvoiceState.Closed;
                    _invoiceRepository.UpdateAsync(item);
                }
            }
        }
    }
}