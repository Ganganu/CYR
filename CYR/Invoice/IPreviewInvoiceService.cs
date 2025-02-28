using CYR.Services;

namespace CYR.Invoice
{
    public interface IPreviewInvoiceService
    {
        INavigationService NavigationService { get; }

        Task SaveInvoice(CreateInvoiceModel createInvoiceModel);
    }
}