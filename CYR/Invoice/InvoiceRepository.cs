
namespace CYR.Invoice
{
    public class InvoiceRepository : IInvoiceRepository
    {
        public InvoiceRepository()
        {

        }

        public Task DeleteAsync(InvoiceModel invoice)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InvoiceModel>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InvoiceModel>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(InvoiceModel invoice)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(InvoiceModel invoice)
        {
            throw new NotImplementedException();
        }
    }
}
