using CYR.Core;
using System.Data.SQLite;

namespace CYR.Logging;

public class LoggingRepository
{
    private readonly IDatabaseConnection _databaseConnection;
    public LoggingRepository(IDatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<bool> InsertInTransactionAsync(HisModel his, SQLiteTransaction transaction)
    {
        bool succes = false;

        string query = @"insert into his (logging_type,user_id,address_id,client_id,invoice_id,order_item_id,unit_of_measure_id,message)
                         values(@logging_type,@user_id,@address_id,@client_id,@invoice_id,@order_item_id,@unit_of_measure_id,@message)";

        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            {"@logging_type",his.LoggingType },
            {"user_id", his.UserId },
            {"address_id", his.AddressId },
            {"client_id", his.ClientId },
            {"invoice_id", his.InvoiceId },
            {"order_item_id", his.OrderItemId },
            {"unit_of_measure_id", his.UnitOfMeasureId },
            {"message", his.Message },
        };
        int affectedRows = await _databaseConnection.ExecuteNonQueryInTransactionAsync(transaction, query, parameters);
        return succes = affectedRows > 0;
    }
    public async Task<bool> InsertAsync(HisModel his)
    {
        bool succes = false;

        string query = @"insert into his (logging_type,user_id,address_id,client_id,invoice_id,order_item_id,unit_of_measure_id,message)
                         values(@logging_type,@user_id,@address_id,@client_id,@invoice_id,@order_item_id,@unit_of_measure_id,@message)";

        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            {"@logging_type",his.LoggingType },
            {"user_id", his.UserId },
            {"address_id", his.AddressId },
            {"client_id", his.ClientId },
            {"invoice_id", his.InvoiceId },
            {"order_item_id", his.OrderItemId },
            {"unit_of_measure_id", his.UnitOfMeasureId },
            {"message", his.Message },
        };
        int affectedRows = await _databaseConnection.ExecuteNonQueryAsync(query, parameters);
        return succes = affectedRows > 0;
    }
}
