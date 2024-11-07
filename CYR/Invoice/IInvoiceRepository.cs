using System.Data.SQLite;

namespace CYR.Invoice
{
    public interface IInvoiceRepository
    {
        Task DeleteAsync(InvoiceModel invoice);
        Task<IEnumerable<InvoiceModel>> GetAllAsync();
        Task<IEnumerable<InvoiceModel>> GetByIdAsync(int id);
        Task InsertAsync(InvoiceModel invoice, SQLiteTransaction? transaction = null);
        Task UpdateAsync(InvoiceModel invoice);
        Task<IEnumerable<InvoicePositionModel>> GetAllPositionsByInvoiceIdAsync(int invoiceId);
    }
}
