
using CYR.Core;
using System.Data.SQLite;

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

        public async Task InsertAsync(InvoicePositionModel invoicePosition, SQLiteTransaction? transaction = null)
        {
            string query = "INSERT INTO Rechnungspositionen (Rechnungsnummer,Beschreibung,Menge," +
                "Einheit, Einheitspreis) VALUES (@Rechnungsnummer,@Beschreibung,@Menge," +
                "@Einheit, @Einheitspreis)";
            Dictionary<string, object> queryParameters = new Dictionary<string, object>
            {
                {"Position_ID",invoicePosition.InvoiceNumber},
                {"Rechnungsnummer",invoicePosition.InvoiceNumber},
                {"Beschreibung",invoicePosition.Description},
                {"Menge",invoicePosition.Quantity},
                {"Einheit",invoicePosition.UnitOfMeasure},
                {"Einheitspreis",invoicePosition.UnitPrice}
            };
            if (transaction != null)
            {
                await _databaseConnection.ExecuteNonQueryInTransactionAsync(transaction, query, queryParameters);
            }
            else
            {
                await _databaseConnection.ExecuteNonQueryAsync(query, queryParameters);
            }
        }

        public Task UpdateAsync(InvoicePositionModel invoicePosition)
        {
            throw new NotImplementedException();
        }
    }
}
