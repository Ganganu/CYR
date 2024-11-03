
using CYR.Core;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace CYR.Invoice
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly IDatabaseConnection _databaseConnection;

        public InvoiceRepository(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
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

        public async Task InsertAsync(InvoiceModel invoice, SQLiteTransaction? transaction = null)
        {
            string query = "INSERT INTO Rechnungen (Rechnungsnummer,Kundennummer,Rechnungsdatum,Fälligkeitsdatum," +
                "Nettobetrag, Bruttobetrag,Paragraf,Status,Betreff,Objektnummer) VALUES (@Rechnungsnummer,@Kundennummer,@Rechnungsdatum,@Fälligkeitsdatum," +
                "@Nettobetrag, @Bruttobetrag,@Paragraf,@Status,@Betreff,@Objektnummer)";
            Dictionary<string, object> queryParameters = new Dictionary<string, object>
            {
                {"Rechnungsnummer",invoice.InvoiceNumber },
                {"Kundennummer",invoice.Customer.ClientNumber },
                {"Rechnungsdatum",invoice.IssueDate },
                {"Fälligkeitsdatum",invoice.DueDate},
                {"Nettobetrag",invoice.NetAmount },
                {"Bruttobetrag",invoice.GrossAmount },
                {"Paragraf",invoice.Paragraph },
                {"Status",invoice.State },
                {"Betreff",invoice.Subject},
                {"Objektnummer",invoice.ObjectNumber}
            };
            if (transaction != null)
            {
                await _databaseConnection.ExecuteNonQueryInTransactionAsync(transaction, query, queryParameters);
            }
            else
            {
                await _databaseConnection.ExecuteNonQueryAsync(query,queryParameters);
            }
        }

        public Task UpdateAsync(InvoiceModel invoice)
        {
            throw new NotImplementedException();
        }
    }
}
