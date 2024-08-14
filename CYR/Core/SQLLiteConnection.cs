using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYR.Core
{
    public class SQLLiteConnection : IDatabaseConnection
    {
        private string _connectionString;
        public SQLLiteConnection(string connectionString)
        {
            _connectionString = connectionString;
        }
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
    }
}
