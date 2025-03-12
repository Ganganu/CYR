using System.Data.SQLite;
using CYR.Invoice.Model;

namespace CYR.Invoice.Repository
{
    public interface IInvoiceRepository
    {
        Task DeleteAsync(InvoiceModel invoice);
        Task<IEnumerable<InvoiceModel>> GetAllAsync();
        Task<InvoiceModel> GetByIdAsync(int id);
        Task InsertAsync(InvoiceModel invoice, SQLiteTransaction? transaction = null);
        Task UpdateAsync(InvoiceModel invoice);
    }
}
