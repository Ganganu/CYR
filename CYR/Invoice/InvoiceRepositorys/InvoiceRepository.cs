using CYR.Clients;
using CYR.Core;
using CYR.Invoice.InvoiceModels;
using System.Data.Common;
using System.Data.SQLite;

namespace CYR.Invoice.InvoiceRepositorys
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly IDatabaseConnection _databaseConnection;

        public InvoiceRepository(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task<bool> DeleteAsync(InvoiceModel invoice)
        {
            bool succes = false;
            await _databaseConnection.ExecuteTransactionAsync(async (transaction) =>
            {
                string deletePositionsQuery = "delete from Rechnungspositionen where Rechnungsnummer = @Rechnungsnummer";
                var deletePositionsParams = new Dictionary<string, object>
                {
                    { "@Rechnungsnummer", invoice.InvoiceNumber }
                };
                await _databaseConnection.ExecuteNonQueryInTransactionAsync(transaction, deletePositionsQuery, deletePositionsParams);

                string deleteInvoiceQuery = "delete from Rechnungen where Rechnungsnummer = @Rechnungsnummer";
                var deleteInvoiceParams = new Dictionary<string, object>
                {
                    { "@Rechnungsnummer", invoice.InvoiceNumber }
                };
                int clientAffectedRows = await _databaseConnection.ExecuteNonQueryInTransactionAsync(transaction, deleteInvoiceQuery, deleteInvoiceParams);
                succes = clientAffectedRows > 0;
            });
            return succes;
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
                    invoice.IssueDate = Convert.ToDateTime(reader["Rechnungsdatum"]);
                    invoice.DueDate = Convert.ToDateTime(reader["Fälligkeitsdatum"]);
                    invoice.NetAmount = Convert.ToDecimal(reader["Nettobetrag"]);
                    invoice.GrossAmount = Convert.ToDecimal(reader["Bruttobetrag"]);
                    if (Enum.TryParse<InvoiceState>(reader["Status"].ToString(), out var state))
                    {
                        invoice.State = state;
                    }
                    else
                    {
                        invoice.State = InvoiceState.Closed;
                    }
                    invoiceList.Add(invoice);
                }
                return invoiceList;
            }
        }
        public async Task<InvoiceModel> GetByIdAsync(int id)
        {
            InvoiceModel invoiceModel = new();
            string query = "SELECT * FROM Rechnungen " +
                "INNER JOIN Kunden ON Rechnungen.Kundennummer = Kunden.Kundennummer " +
                "INNER JOIN Adresse ON Adresse.Kundennummer = Kunden.Kundennummer WHERE Rechnungsnummer = @Rechnungsnummer";
            Dictionary<string, object> queryParameters = new()
            {
                {"Rechnungsnummer",id }
            };
            using (DbDataReader reader = (DbDataReader)await _databaseConnection.ExecuteSelectQueryAsync(query, queryParameters))
            {
                if (await reader.ReadAsync())
                {
                    invoiceModel.InvoiceNumber = Convert.ToInt32(reader["Rechnungsnummer"]);
                    invoiceModel.Customer = new Client();
                    invoiceModel.Customer.Street = reader["Strasse"].ToString();
                    invoiceModel.Customer.PLZ = reader["PLZ"].ToString();
                    invoiceModel.Customer.City = reader["Ort"].ToString();
                    invoiceModel.Customer.ClientNumber = reader["Kundennummer"].ToString();
                    invoiceModel.Customer.Name = reader["Name"].ToString();
                    invoiceModel.IssueDate = Convert.ToDateTime(reader["Rechnungsdatum"]);
                    invoiceModel.DueDate = Convert.ToDateTime(reader["Fälligkeitsdatum"]);
                    invoiceModel.NetAmount = Convert.ToDecimal(reader["Nettobetrag"]);
                    invoiceModel.GrossAmount = Convert.ToDecimal(reader["Bruttobetrag"]);
                    invoiceModel.CommentsTop = reader["commentstop"].ToString();
                    invoiceModel.CommentsBottom = reader["commentsbottom"].ToString();
                    if (invoiceModel.GrossAmount > invoiceModel.NetAmount)
                    {
                        invoiceModel.IsMwstApplicable = true;
                    }
                }
            }
            return invoiceModel;
        }

        public async Task InsertAsync(InvoiceModel invoice, SQLiteTransaction? transaction = null)
        {
            string query = "INSERT INTO Rechnungen (Rechnungsnummer,Kundennummer,Rechnungsdatum,Fälligkeitsdatum," +
                "Nettobetrag, Bruttobetrag,Status, commentstop, commentsbottom) " +
                "VALUES (@Rechnungsnummer,@Kundennummer,@Rechnungsdatum,@Fälligkeitsdatum," +
                "@Nettobetrag, @Bruttobetrag,@Status,@commentstop, @commentsbottom)";

            Dictionary<string, object> queryParameters = new Dictionary<string, object>
            {
                {"Rechnungsnummer",invoice.InvoiceNumber },
                {"Kundennummer",invoice.Customer.ClientNumber },
                {"Rechnungsdatum",invoice.IssueDate },
                {"Fälligkeitsdatum",invoice.DueDate},
                {"Nettobetrag",invoice.NetAmount },
                {"Bruttobetrag",invoice.GrossAmount },
                {"Status",invoice.State },
                {"commentstop", invoice.CommentsTop},
                {"commentsbottom", invoice.CommentsBottom}
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

        public async Task UpdateAsync(InvoiceModel invoice)
        {
            string query = @"
                                UPDATE Rechnungen 
                                SET Kundennummer = @Kundennummer, Rechnungsdatum = @Rechnungsdatum, Fälligkeitsdatum = @Fälligkeitsdatum, 
                                    Nettobetrag = @Nettobetrag, Bruttobetrag = @Bruttobetrag, Status = @Status,  
                                    commentstop = @commentstop, commentsbottom = @commentsbottom
                                WHERE Rechnungsnummer = @Rechnungsnummer";

            Dictionary<string, object> queryParameters = new Dictionary<string, object>
            {
                {"Rechnungsnummer",invoice.InvoiceNumber },
                {"Kundennummer",invoice.Customer.ClientNumber },
                {"Rechnungsdatum",invoice.IssueDate },
                {"Fälligkeitsdatum",invoice.DueDate},
                {"Nettobetrag",invoice.NetAmount },
                {"Bruttobetrag",invoice.GrossAmount },
                {"Status", (int)invoice.State },
                {"commentstop", invoice.CommentsTop},
                {"commentsbottom", invoice.CommentsBottom}

            };

            await _databaseConnection.ExecuteNonQueryAsync(query, queryParameters);
        }

        public async Task<bool> UpdateInvoiceAndPositions(InvoiceModel invoice)
        {
            decimal? nettAmount = 0;
            decimal? grossAmount = 0;
            bool succes = false;
            await _databaseConnection.ExecuteTransactionAsync(async (transaction) =>
            {
                string deletePositionsQuery = "delete from Rechnungspositionen where Rechnungsnummer = @Rechnungsnummer";
                var deletePositionsParams = new Dictionary<string, object>
                {
                    { "@Rechnungsnummer", invoice.InvoiceNumber }
                };
                await _databaseConnection.ExecuteNonQueryInTransactionAsync(transaction, deletePositionsQuery, deletePositionsParams);

                foreach (var pos in invoice.Items)
                {
                    string insertNewPositions = "INSERT INTO Rechnungspositionen (Rechnungsnummer,Beschreibung,Menge,Einheit,Einheitspreis)" +
                     " VALUES (@Rechnungsnummer,@Beschreibung,@Menge,@Einheit,@Einheitspreis)";
                    var insertNewPositionsParams = new Dictionary<string, object>
                    {
                        { "@Rechnungsnummer", invoice.InvoiceNumber },
                        { "@Beschreibung", pos.OrderItem.Description },
                        { "@Menge", pos.Quantity },
                        { "@Einheit", pos.UnitOfMeasure },
                        { "@Einheitspreis", pos.TotalPrice }
                    };
                    nettAmount += pos.TotalPrice;
                    await _databaseConnection.ExecuteNonQueryInTransactionAsync(transaction, insertNewPositions, insertNewPositionsParams);
                }
                if (invoice.IsMwstApplicable && nettAmount.HasValue)
                {
                    grossAmount = nettAmount.Value * 1.19m;
                }
                else
                {
                    grossAmount = nettAmount;
                }
                string updateInvoice = "update Rechnungen set Kundennummer = @Kundennummer, Rechnungsdatum = @Rechnungsdatum, Fälligkeitsdatum = @Fälligkeitsdatum," +
                "Nettobetrag = @Nettobetrag, Bruttobetrag = @Bruttobetrag, Status = @Status,commentstop = @commentstop, commentsbottom = @commentsbottom  " +
                " where Rechnungsnummer = @Rechnungsnummer";
                var updateInvoiceQueryParameters = new Dictionary<string, object>
                {
                    {"Rechnungsnummer",invoice.InvoiceNumber },
                    {"Kundennummer",invoice.Customer.ClientNumber },
                    {"Rechnungsdatum",invoice.IssueDate },
                    {"Fälligkeitsdatum",invoice.DueDate},
                    {"Nettobetrag",nettAmount },
                    {"Bruttobetrag",grossAmount },
                    {"Status", (int)invoice.State },
                    {"commentstop", invoice.CommentsTop},
                    {"commentsbottom", invoice.CommentsBottom}
                };
                int clientAffectedRows = await _databaseConnection.ExecuteNonQueryInTransactionAsync(transaction, updateInvoice, updateInvoiceQueryParameters);
                succes = clientAffectedRows > 0;
            });
            return succes;
        }
    }
}
