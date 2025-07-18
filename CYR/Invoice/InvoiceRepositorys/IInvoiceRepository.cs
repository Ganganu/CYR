using System.Data.SQLite;
using CYR.Invoice.InvoiceModels;
using CYR.Messages;

namespace CYR.Invoice.InvoiceRepositorys
{
    public interface IInvoiceRepository
    {
        Task<bool> DeleteAsync(InvoiceModel invoice);
        Task<IEnumerable<InvoiceModel>> GetAllAsync();
        Task<InvoiceModel> GetByIdAsync(int id);
        Task InsertAsync(InvoiceModel invoice, SQLiteTransaction? transaction = null);
        Task UpdateAsync(InvoiceModel invoice);
        Task<SnackbarMessage> UpdateInvoiceAndPositions(InvoiceModel invoice);
    }
}
