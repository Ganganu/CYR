using System.Data;
using System.Data.SQLite;
using System.Diagnostics;

namespace CYR.Core
{
    public class SQLLiteConnection : IDatabaseConnection
    {
        private string _connectionString;
        public SQLLiteConnection(string connectionString)
        {
            _connectionString = connectionString;
        }
        public string ConnectionString => _connectionString;

        public async Task<IDataReader> ExecuteSelectQueryAsync(string query)
        {
            IDataReader? reader = null;
            SQLiteConnection sqlConnection = new SQLiteConnection(_connectionString);
            SQLiteCommand sqlCommand = new SQLiteCommand(query, sqlConnection);

            try
            {
                await sqlConnection.OpenAsync();
                reader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }
            catch (SQLiteException ex)
            {
                Debug.Write(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
            return reader;
        }

        public async Task<IDataReader> ExecuteSelectQueryAsync(string query, Dictionary<string, object> parameters)
        {
            IDataReader? reader = null;
            SQLiteConnection sqlConnection = new SQLiteConnection(_connectionString);

            sqlConnection.Open();
            using (SQLiteCommand sqlCommand = new SQLiteCommand(query, sqlConnection))
            {
                foreach (var parameter in parameters)
                {
                    sqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
                reader = await sqlCommand.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }

            return reader;
        }
        public async Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object> parameters)
        {
            int affectedRows = 0;
            using (SQLiteConnection sqlConnection = new SQLiteConnection(_connectionString))
            {
                await sqlConnection.OpenAsync();
                using (SQLiteCommand sqlCommand = new SQLiteCommand(query, sqlConnection))
                {
                    foreach (var parameter in parameters)
                    {
                        sqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }
                    affectedRows = await sqlCommand.ExecuteNonQueryAsync();
                }
                sqlConnection.Close();
            }
            return affectedRows;
        }
        public async Task<bool> ExecuteSelectQueryAsyncCheck(string query, Dictionary<string, object> parameters)
        {
            using (SQLiteConnection sqlConnection = new SQLiteConnection(_connectionString))
            {
                await sqlConnection.OpenAsync();
                using (SQLiteCommand sqlCommand = new SQLiteCommand(query, sqlConnection))
                {
                    foreach (var parameter in parameters)
                    {
                        sqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }

                    using (SQLiteDataReader reader = (SQLiteDataReader)await sqlCommand.ExecuteReaderAsync())
                    {
                        // Return true if there is at least one record
                        return await reader.ReadAsync();
                    }
                }
            }
        }

        public async Task<int> ExecuteNonQueryInTransactionAsync(SQLiteTransaction transaction, string query, Dictionary<string, object> parameters)
        {
            var connection = transaction.Connection;
            using (var command = new SQLiteCommand(query, connection, transaction))
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
                return await command.ExecuteNonQueryAsync();
            }
        }
    }
}
