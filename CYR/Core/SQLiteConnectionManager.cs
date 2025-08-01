using System.Data;
using System.Data.SQLite;

namespace CYR.Core;

public class SQLiteConnectionManager : IDisposable, IDatabaseConnection
{
    private readonly string _connectionString;
    private SQLiteConnection? _connection;

    public SQLiteConnectionManager(string connectionString)
    {
        _connectionString = connectionString;
    }

    public string ConnectionString => _connectionString;

    private async Task<SQLiteConnection> GetOpenConnectionAsync()
    {
        if (_connection == null || _connection.State != ConnectionState.Open)
        {
            _connection = new SQLiteConnection(_connectionString);
            await _connection.OpenAsync();
        }
        return _connection;
    }

    private async Task<SQLiteCommand> CreateCommandWithParametersAsync(string query, Dictionary<string, object>? parameters)
    {
        var connection = await GetOpenConnectionAsync();
        var command = new SQLiteCommand(query, connection);

        if (parameters != null)
        {
            foreach (var parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }
        }
        return command;
    }

    public async Task<IDataReader> ExecuteSelectQueryAsync(string query, Dictionary<string, object>? parameters = null)
    {
        var command = await CreateCommandWithParametersAsync(query, parameters);
        return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
    }
      
    public async Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object> parameters)
    {
        using var command = await CreateCommandWithParametersAsync(query, parameters);
        var connection = command.Connection;
        try
        {
            var affectedRows = await command.ExecuteNonQueryAsync();
            return affectedRows;
        }
        finally
        {
            connection?.Close();
        }
    }

    public async Task<T?> ExecuteScalarAsync<T>(string query, Dictionary<string, object>? parameters = null)
    {
        using var command = await CreateCommandWithParametersAsync(query, parameters);
        var result = await command.ExecuteScalarAsync();
        if (result == DBNull.Value || result == null)
        {
            return default(T);
        }
        return (T)Convert.ChangeType(result, typeof(T));
    }

    public async Task<int> ExecuteNonQueryInTransactionAsync(SQLiteTransaction transaction, string query, Dictionary<string, object> parameters)
    {
        using var command = new SQLiteCommand(query, transaction.Connection, transaction);
        foreach (var parameter in parameters)
        {
            command.Parameters.AddWithValue(parameter.Key, parameter.Value);
        }
        return await command.ExecuteNonQueryAsync();
    }

    public async Task<IDataReader> ExecuteReaderInTransactionAsync(SQLiteTransaction transaction, string query, Dictionary<string, object> parameters)
    {
        var connection = transaction.Connection;
        using (var command = new SQLiteCommand(query, connection, transaction))
        {
            foreach (var parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }
            // CommandBehavior.SingleResult is a good option here, as we expect one result set.
            // DO NOT use CommandBehavior.CloseConnection here, as it would close the transaction's connection.
            return await command.ExecuteReaderAsync(CommandBehavior.SingleResult);
        }
    }

    public async Task ExecuteTransactionAsync(Func<SQLiteTransaction, Task> transactionOperations)
    {
        var connection = await GetOpenConnectionAsync();
        var transaction = connection.BeginTransaction();
        try
        {
            await transactionOperations(transaction);
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
        finally
        {
            transaction.Dispose();
            connection.Close();
            _connection = null;
        }
    }

    public void Dispose()
    {
        if (_connection != null)
        {
            _connection.Dispose();
            _connection = null;
        }
    }
}