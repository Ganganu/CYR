using System.Data;

namespace CYR.Core
{
    public interface IDatabaseConnection
    {
        Task<IDataReader> ExecuteSelectQueryAsync(string query);
        Task<IDataReader> ExecuteSelectQueryAsync(string query,Dictionary<string, object> parameters);
        Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object> parameters);
        Task<bool> ExecuteSelectQueryAsyncCheck(string query, Dictionary<string, object> parameters);
    }
}