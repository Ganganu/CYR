using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private readonly InvoiceActionsViewModel _invoiceActionsViewModel;

        public InvoiceListViewModel(IInvoiceRepository invoiceRepository,
            INavigationService navigationService, InvoiceActionsViewModel invoiceActionsViewModel)
        {
            _invoiceRepository = invoiceRepository;
            NavigationService = navigationService;
            _invoiceActionsViewModel = invoiceActionsViewModel;
            Initialize();
        }
        private async void Initialize()
        {
            IEnumerable<InvoiceModel> invoices = await _invoiceRepository.GetAllAsync(); 
            Invoices = [.. invoices];
            IsInvoiceActionViewVisible = Visibility.Collapsed;
        }
        [ObservableProperty]
        private Visibility? _isInvoiceActionViewVisible;

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
