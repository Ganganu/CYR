namespace CYR.Invoice
{
    public interface IInvoicePositionRepository
    {
        Task DeleteAsync(InvoicePositionModel invoicePosition);
        Task<IEnumerable<InvoicePositionModel>> GetAllAsync();
        Task<IEnumerable<InvoicePositionModel>> GetByIdAsync(int id);
        Task InsertAsync(InvoicePositionModel invoicePosition);
        Task UpdateAsync(InvoicePositionModel invoicePosition);
    }
}
