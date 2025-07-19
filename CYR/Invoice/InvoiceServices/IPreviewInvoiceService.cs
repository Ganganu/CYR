using CYR.Invoice.InvoiceModels;
using CYR.Messages;

namespace CYR.Invoice.InvoiceServices;

public interface IPreviewInvoiceService
{
    Task<SnackbarMessage> PreviewInvoice(CreateInvoiceModel createInvoiceModel);
}