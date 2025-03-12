using CYR.Invoice.Model;
using CYR.Services;

namespace CYR.Invoice.Repository
{
    public interface ISaveInvoiceInvoicePositionService
    {
        INavigationService NavigationService { get; }

        Task SaveInvoice(CreateInvoiceModel createInvoiceModel);
    }
}