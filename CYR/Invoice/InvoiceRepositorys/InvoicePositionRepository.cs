using System.Data.Common;
using System.Data.SQLite;
using CYR.Core;
using CYR.Invoice.InvoiceModels;
using CYR.User;

namespace CYR.Invoice.InvoiceRepositorys;

public class InvoicePositionRepository : IInvoicePositionRepository
{
    private readonly IDatabaseConnection _databaseConnection;
    private readonly UserContext _userContext;

    public InvoicePositionRepository(IDatabaseConnection databaseConnection, UserContext userContext)
    {
        _databaseConnection = databaseConnection;
        _userContext = userContext;
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
        List<InvoicePositionModel> invoicePositions = new();
        InvoicePositionModel invoicePosition;
        string query = "SELECT * FROM Rechnungspositionen INNER JOIN Rechnungen " +
            $"ON Rechnungspositionen.Rechnungsnummer = Rechnungen.Rechnungsnummer WHERE Rechnungen.Rechnungsnummer LIKE {invoiceId} and Rechnungen.user_id = {_userContext.CurrentUser.Id}";

        using (DbDataReader reader = (DbDataReader)await _databaseConnection.ExecuteSelectQueryAsync(query))
        {
            while (await reader.ReadAsync())
            {
                invoicePosition = new InvoicePositionModel();
                invoicePosition.Quantity = reader["Menge"].ToString();
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
            "Einheit, Einheitspreis, user_id) VALUES (@Rechnungsnummer,@Beschreibung,@Menge," +
            "@Einheit, @Einheitspreis, @user_id)";
        var convertedQuantity = Convert.ToDecimal(invoicePosition.Quantity);
        Dictionary<string, object> queryParameters = new Dictionary<string, object>
        {
            {"Position_ID",invoicePosition.InvoiceNumber},
            {"Rechnungsnummer",invoicePosition.InvoiceNumber},
            {"Beschreibung",invoicePosition.Description},
            {"Menge",convertedQuantity},
            {"Einheit",invoicePosition.UnitOfMeasure},
            {"Einheitspreis",invoicePosition.UnitPrice},
            {"user_id",_userContext.CurrentUser.Id}
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
