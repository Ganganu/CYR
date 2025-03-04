using CYR.Invoice.Model;
using CYR.Services;

namespace CYR.Invoice.Service
{
    public interface IPreviewInvoiceService
    {
        INavigationService NavigationService { get; }

        Task SaveInvoice(CreateInvoiceModel createInvoiceModel);
    }
}