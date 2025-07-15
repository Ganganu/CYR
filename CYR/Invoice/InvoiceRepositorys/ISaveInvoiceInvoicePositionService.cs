using CYR.Invoice.InvoiceModels;
using CYR.Services;

namespace CYR.Invoice.InvoiceRepositorys
{
    public interface ISaveInvoiceInvoicePositionService
    {
        Task SaveInvoice(CreateInvoiceModel createInvoiceModel);
    }
}