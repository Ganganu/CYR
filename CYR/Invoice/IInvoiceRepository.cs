namespace CYR.Invoice
{
    public interface IInvoiceRepository
    {
        Task DeleteAsync(InvoiceModel invoice);
        Task<IEnumerable<InvoiceModel>> GetAllAsync();
        Task<IEnumerable<InvoiceModel>> GetByIdAsync(int id);
        Task InsertAsync(InvoiceModel invoice);
        Task UpdateAsync(InvoiceModel invoice);
    }
}
