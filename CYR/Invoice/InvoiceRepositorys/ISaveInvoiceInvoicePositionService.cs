using CYR.Invoice.InvoiceModels;
using CYR.Services;

namespace CYR.Invoice.InvoiceRepositorys
{
    public interface ISaveInvoiceInvoicePositionService
    {
        INavigationService NavigationService { get; }

        Task SaveInvoice(CreateInvoiceModel createInvoiceModel);
    }
}