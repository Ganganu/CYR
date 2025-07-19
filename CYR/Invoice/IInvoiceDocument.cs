using CYR.Invoice.InvoiceModels;
using QuestPDF.Infrastructure;

namespace CYR.Invoice;

public interface IInvoiceDocument : IDocument
{
    public InvoiceModel Model { get; set; }
}