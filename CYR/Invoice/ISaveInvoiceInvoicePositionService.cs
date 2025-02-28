using CYR.Services;

namespace CYR.Invoice
{
    public interface ISaveInvoiceInvoicePositionService
    {
        INavigationService NavigationService { get; }

        Task SaveInvoice(CreateInvoiceModel createInvoiceModel);
    }
}