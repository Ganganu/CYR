using System.Data.SQLite;
using CYR.Invoice.Model;

namespace CYR.Invoice.Repository
{
    public interface IInvoicePositionRepository
    {
        Task DeleteAsync(InvoicePositionModel invoicePosition);
        Task<IEnumerable<InvoicePositionModel>> GetAllAsync();
        Task<IEnumerable<InvoicePositionModel>> GetByIdAsync(int id);
        Task InsertAsync(InvoicePositionModel invoicePosition, SQLiteTransaction? transaction = null);
        Task UpdateAsync(InvoicePositionModel invoicePosition);
        Task<IEnumerable<InvoicePositionModel>> GetAllPositionsByInvoiceIdAsync(int? invoiceId);
    }
}
