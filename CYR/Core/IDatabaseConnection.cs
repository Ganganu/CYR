using System.Data;
using System.Data.SQLite;

namespace CYR.Core
{
    public interface IDatabaseConnection
    {
        string ConnectionString { get; }

        void Dispose();
        Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object> parameters);
        Task<int> ExecuteNonQueryInTransactionAsync(SQLiteTransaction transaction, string query, Dictionary<string, object> parameters);
        Task<IDataReader> ExecuteReaderInTransactionAsync(SQLiteTransaction transaction, string query, Dictionary<string, object> parameters);
        Task<T?> ExecuteScalarAsync<T>(string query, Dictionary<string, object>? parameters = null);
        Task<IDataReader> ExecuteSelectQueryAsync(string query, Dictionary<string, object>? parameters = null);
        Task ExecuteTransactionAsync(Func<SQLiteTransaction, Task> transactionOperations);
    }
}