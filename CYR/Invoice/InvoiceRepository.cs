using CYR.Clients;
using CYR.Core;
using System.Data.Common;
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

        public async Task<IEnumerable<InvoiceModel>> GetAllAsync()
        {
            List<InvoiceModel> invoiceList = new List<InvoiceModel>();
            InvoiceModel invoice;
            string query = "SELECT * FROM Rechnungen " +
                "INNER JOIN Kunden ON Rechnungen.Kundennummer = Kunden.Kundennummer " +
                "INNER JOIN Adresse ON Adresse.Kundennummer = Kunden.Kundennummer";
            using (DbDataReader reader = (DbDataReader)await _databaseConnection.ExecuteSelectQueryAsync(query))
            {
                while (await reader.ReadAsync())
                {
                    invoice = new InvoiceModel();
                    invoice.InvoiceNumber = Convert.ToInt32(reader["Rechnungsnummer"]);
                    invoice.Customer = new Client();
                    invoice.Customer.Street = reader["Strasse"].ToString();
                    invoice.Customer.PLZ = reader["PLZ"].ToString();
                    invoice.Customer.City = reader["Ort"].ToString();
                    invoice.Customer.ClientNumber = reader["Kundennummer"].ToString();
                    invoice.Customer.Name = reader["Name"].ToString();
                    invoice.IssueDate = reader["Rechnungsdatum"].ToString();
                    invoice.DueDate = reader["Fälligkeitsdatum"].ToString();
                    invoice.NetAmount = Convert.ToDecimal(reader["Nettobetrag"]);
                    invoice.GrossAmount = Convert.ToDecimal(reader["Bruttobetrag"]);
                    invoice.Subject = reader["Betreff"].ToString();
                    invoice.ObjectNumber = reader["Objektnummer"].ToString();
                    //invoice.State = (InvoiceState)reader["Status"];
                    invoiceList.Add(invoice);
                }
                return invoiceList;
            }
        }
        public Task<IEnumerable<InvoiceModel>> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task InsertAsync(InvoiceModel invoice, SQLiteTransaction? transaction = null)
        {
            string query = "INSERT INTO Rechnungen (Rechnungsnummer,Kundennummer,Rechnungsdatum,Fälligkeitsdatum," +
                "Nettobetrag, Bruttobetrag,Paragraf,Status,Betreff,Objektnummer,Zeitraum) VALUES (@Rechnungsnummer,@Kundennummer,@Rechnungsdatum,@Fälligkeitsdatum," +
                "@Nettobetrag, @Bruttobetrag,@Paragraf,@Status,@Betreff,@Objektnummer,@Zeitraum)";
            string executionDate = $"{invoice.StartDate}-{invoice.EndDate}";
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
                {"Objektnummer",invoice.ObjectNumber},
                {"Zeitraum", invoice.StartDate}
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

        public Task UpdateAsync(InvoiceModel invoice)
        {
            throw new NotImplementedException();
        }        
    }
}
