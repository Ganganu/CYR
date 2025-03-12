using CYR.Invoice.InvoiceModels;

namespace CYR.Invoice.InvoiceServices
{
    public interface IPreviewInvoiceService
    {
        Task SaveInvoice(CreateInvoiceModel createInvoiceModel);
    }
}