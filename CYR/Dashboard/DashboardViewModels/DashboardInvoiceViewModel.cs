using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CYR.Core;
using CYR.Invoice.InvoiceModels;
using CYR.Invoice.InvoiceRepositorys;
using CYR.Invoice.InvoiceViewModels;
using CYR.Services;

namespace CYR.Dashboard.DashboardViewModels;

public partial class DashboardInvoiceViewModel : ObservableRecipient
{
    private readonly IInvoiceRepository _invoiceRepository;
    public DashboardInvoiceViewModel(IInvoiceRepository invoiceRepository, INavigationService navigation)
    {
        _invoiceRepository = invoiceRepository;
        Initialize();
        _navigation = navigation;
    }

    [ObservableProperty]
    private List<InvoiceModel> _invoiceModels;
    [ObservableProperty]
    private INavigationService _navigation;

    private async Task Initialize()
    {
        var invoices = await _invoiceRepository.GetAllAsync();
        if (invoices is null) return;
        InvoiceModels = [.. invoices.OrderByDescending(i => i.IssueDate ?? DateTime.MinValue).Take(5)];        
    }

    [RelayCommand]
    private void NavigateToInvoiceList()
    {
        Navigation.NavigateTo<InvoiceListViewModel>();
        Messenger.Send(new NavigateToInvoiceEvent());
    }
    [RelayCommand]
    private void NavigateToCreateInvoice()
    {
        Navigation.NavigateTo<CreateInvoiceViewModel>();
        Messenger.Send(new NavigateToInvoiceEvent());
    }
}
