using CYR.Invoice.InvoiceModels;
using CYR.Messages;

namespace CYR.Invoice.InvoiceRepositorys;

public interface ISaveInvoiceInvoicePositionService
{
    Task<SnackbarMessage> SaveInvoice(CreateInvoiceModel createInvoiceModel);
}