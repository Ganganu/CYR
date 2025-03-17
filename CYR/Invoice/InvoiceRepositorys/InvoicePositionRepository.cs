using CYR.Core;
using CYR.Invoice.InvoiceModels;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;

namespace CYR.Invoice.InvoiceRepositorys
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

        public async Task<IEnumerable<InvoicePositionModel>> GetAllPositionsByInvoiceIdAsync(int? invoiceId)
        {
            List<InvoicePositionModel> invoicePositions = new List<InvoicePositionModel>();
            InvoicePositionModel invoicePosition;
            string query = "SELECT * FROM Rechnungspositionen INNER JOIN Rechnungen " +
                $"ON Rechnungspositionen.Rechnungsnummer = Rechnungen.Rechnungsnummer WHERE Rechnungen.Rechnungsnummer LIKE {invoiceId}";

            using (DbDataReader reader = (DbDataReader)await _databaseConnection.ExecuteSelectQueryAsync(query))
            {
                while (await reader.ReadAsync())
                {
                    invoicePosition = new InvoicePositionModel();
                    string test = reader["Menge"].ToString();
                    invoicePosition.Quantity = Convert.ToDecimal(reader["Menge"], CultureInfo.InvariantCulture);
                    invoicePosition.UnitOfMeasure = reader["Einheit"].ToString();
                    invoicePosition.Description = reader["Beschreibung"].ToString();
                    invoicePosition.UnitPrice = Convert.ToDecimal(reader["Einheitspreis"]);
                    invoicePosition.TotalPrice = Convert.ToDecimal(reader["Gesamtpreis"]);
                    invoicePositions.Add(invoicePosition);
                }
            }
            return invoicePositions;
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
