using CYR.Invoice.Model;
using CYR.Services;

namespace CYR.Invoice.Service
{
    public interface IPreviewInvoiceService
    {
        Task SaveInvoice(CreateInvoiceModel createInvoiceModel);
    }
}