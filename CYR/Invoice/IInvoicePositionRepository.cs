using System.Data.SQLite;

namespace CYR.Invoice
{
    public interface IInvoicePositionRepository
    {
        Task DeleteAsync(InvoicePositionModel invoicePosition);
        Task<IEnumerable<InvoicePositionModel>> GetAllAsync();
        Task<IEnumerable<InvoicePositionModel>> GetByIdAsync(int id);
        Task InsertAsync(InvoicePositionModel invoicePosition, SQLiteTransaction? transaction = null);
        Task UpdateAsync(InvoicePositionModel invoicePosition);
        Task<IEnumerable<InvoicePositionModel>> GetAllPositionsByInvoiceIdAsync(int invoiceId);
    }
}
