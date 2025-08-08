using CommunityToolkit.Mvvm.ComponentModel;
using CYR.Invoice.InvoiceModels;
using CYR.Invoice.InvoiceRepositorys;

namespace CYR.Dashboard.DashboardViewModels;

public partial class DashboardInvoiceViewModel : ObservableRecipient
{
    private readonly IInvoiceRepository _invoiceRepository;
    public DashboardInvoiceViewModel(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
        Initialize();
    }

    [ObservableProperty]
    private List<InvoiceModel> _invoiceModels;

    private async Task Initialize()
    {
        var invoices = await _invoiceRepository.GetAllAsync();
        if (invoices is null) return;
        InvoiceModels = [.. invoices.OrderByDescending(i => i.IssueDate ?? DateTime.MinValue).Take(5)];
    }

}
