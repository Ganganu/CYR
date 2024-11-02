
using CYR.Core;

namespace CYR.Invoice
{
    public class InvoicePositionRepository : IInvoicePositionRepository
    {
        private readonly IDatabaseConnection _databaseConnection;

        public InvoicePositionRepository(IDatabaseConnection databaseConnection) 
        {
            _databaseConnection = databaseConnection;
        }

        public Task DeleteAsync(InvoicePositionModel invoicePosition)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InvoicePositionModel>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InvoicePositionModel>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task InsertAsync(InvoicePositionModel invoicePosition)
        {
            string query = "INSERT INTO Rechnungen (Rechnungsnummer,Beschreibung,Menge," +
                "Einheit, Einheitspreis,Gesamtpreis) VALUES (@Rechnungsnummer,@Beschreibung,@Menge," +
                "@Einheit, @Einheitspreis,@Gesamtpreis)";
            Dictionary<string, object> queryParameters = new Dictionary<string, object>
            {
                {"Position_ID",invoicePosition.InvoiceNumber},
                {"Rechnungsnummer",invoicePosition.InvoiceNumber},
                {"Beschreibung",invoicePosition.Description},
                {"Menge",invoicePosition.Quantity},
                {"Einheit",invoicePosition.UnitOfMeasure},
                {"Einheitspreis",invoicePosition.UnitPrice},
                {"Gesamtpreis",invoicePosition.TotalPrice}
            };
            int affectedRows = await _databaseConnection.ExecuteNonQueryAsync(query, queryParameters);
        }

        public Task UpdateAsync(InvoicePositionModel invoicePosition)
        {
            throw new NotImplementedException();
        }
    }
}
