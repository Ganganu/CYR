using System.Data;
using System.Data.SQLite;

namespace CYR.Core
{
    public interface IDatabaseConnection
    {
        string ConnectionString {get;}
        Task<IDataReader> ExecuteSelectQueryAsync(string query);
        Task<IDataReader> ExecuteSelectQueryAsync(string query,Dictionary<string, object> parameters);
        Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object> parameters);
        Task<bool> ExecuteSelectQueryAsyncCheck(string query, Dictionary<string, object> parameters);
        Task<int> ExecuteNonQueryInTransactionAsync(SQLiteTransaction transaction, string query, Dictionary<string, object> parameters);
    }
}