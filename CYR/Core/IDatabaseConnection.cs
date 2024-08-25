using System.Data;

namespace CYR.Core
{
    public interface IDatabaseConnection
    {
        Task<IDataReader> ExecuteSelectQueryAsync(string query);
        Task ExecuteSelectQueryAsync(string query,Dictionary<string, object> parameters);
    }
}