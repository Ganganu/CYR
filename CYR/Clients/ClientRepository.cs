using System.Data.Common;
using System.Data.SQLite;
using CYR.Core;
using CYR.User;

namespace CYR.Clients;

public class ClientRepository : IClientRepository
{
    private readonly IDatabaseConnection _connection;
    private readonly UserContext _userContext;

    public ClientRepository(IDatabaseConnection connection, UserContext userContext)
    {
        _connection = connection;
        _userContext = userContext;
    }

    public async Task<bool> DeleteAsync(Client client)
    {
        bool succes = false;
        List<string> invoiceNumbers = new List<string>();

        await _connection.ExecuteTransactionAsync(async (transaction) =>
        {
            string addressQuery = "delete from Adresse where Kundennummer = @Kundennummer and user_id = @user_id";
            var addressParams = new Dictionary<string, object>
            {
                { "@Kundennummer", client.ClientNumber },
                { "@user_id", _userContext.CurrentUser.Id }
            };
            await _connection.ExecuteNonQueryInTransactionAsync(transaction, addressQuery, addressParams);

            //get Rechnungsnummer für Rechnungsposition
            string selectInvoiceNumber = "SELECT r.Rechnungsnummer from Rechnungen as r inner join Kunden as k on r.Kundennummer = k.Kundennummer" +
            " where k.Kundennummer = @Kundennummer and k.user_id = @user_id";
            var selectInvoiceParams = new Dictionary<string, object>
            {
                { "@Kundennummer", client.ClientNumber },
                { "@user_id", _userContext.CurrentUser.Id}
            };
            using (var reader = (SQLiteDataReader)await _connection.ExecuteReaderInTransactionAsync(transaction, selectInvoiceNumber, selectInvoiceParams))
            {
                while (await reader.ReadAsync())
                {
                    invoiceNumbers.Add(reader.GetString(0));
                }
            }
            //delete mehrere Rechnungspositionen wegen FK
            if (invoiceNumbers.Any())
            {
                var parameterNames = invoiceNumbers.Select((_, index) => $"@Rechnungsnummer{index}").ToList();
                string inClause = string.Join(",", parameterNames);
                string orderPositionQuery = $"DELETE FROM Rechnungspositionen WHERE Rechnungsnummer IN ({inClause}) and user_id = @user_id";
                var orderPositionParams = new Dictionary<string, object>
                {
                    { "@user_id", _userContext.CurrentUser.Id }
                };
                for (int i = 0; i < invoiceNumbers.Count; i++)
                {
                    orderPositionParams.Add(parameterNames[i], invoiceNumbers[i]);
                }

                await _connection.ExecuteNonQueryInTransactionAsync(transaction, orderPositionQuery, orderPositionParams);
            }


            //delete Rechnung nachdem Rechnungsposition
            string orderQuery = "DELETE FROM Rechnungen WHERE Kundennummer = @Kundennummer and user_id = @user_id";
            var orderParams = new Dictionary<string, object>
            {
                { "@Kundennummer", client.ClientNumber },
                { "@user_id", _userContext.CurrentUser.Id}
            };
            await _connection.ExecuteNonQueryInTransactionAsync(transaction, orderQuery, orderParams);

            string clientQuery = "DELETE FROM Kunden WHERE Kundennummer = @Kundennummer and user_id = @user_id";
            var clientParams = new Dictionary<string, object>
            {
                { "@Kundennummer", client.ClientNumber },
                { "@user_id", _userContext.CurrentUser.Id}
            };
            int clientAffectedRows = await _connection.ExecuteNonQueryInTransactionAsync(transaction, clientQuery, clientParams);
            succes = clientAffectedRows > 0;
        });

        return succes;
    }

    public async Task<IEnumerable<Client>> GetAllAsync()
    {
        List<Client> clientList = new List<Client>();
        Client client;
        string query = @"select Kunden.Kundennummer, Kunden.Name, Adresse.Strasse, Adresse.PLZ, Adresse.Ort, 
                        Kunden.Telefonnummer, Kunden.Email, Kunden.Erstellungsdatum
                        from Kunden inner join Adresse on Kunden.Kundennummer = Adresse.Kundennummer where Kunden.user_id = @user_id";
        var queryParameters = new Dictionary<string, object>
            {
                { "@user_id", _userContext.CurrentUser.Id}
            };
        using (DbDataReader reader = (DbDataReader)await _connection.ExecuteSelectQueryAsync(query, queryParameters))
        {
            while (await reader.ReadAsync())
            {
                client = new Client();
                client.ClientNumber = reader["Kundennummer"].ToString();
                client.Name = reader["Name"].ToString();
                client.Street = reader["Strasse"].ToString();
                client.PLZ = reader["PLZ"].ToString();
                client.City = reader["Ort"].ToString();
                client.Telefonnumber = reader["Telefonnummer"].ToString();
                client.EmailAddress = reader["Email"].ToString();
                client.CreationDate = reader["Erstellungsdatum"].ToString();
                clientList.Add(client);
            }
        }
        return clientList;
    }

    public Task<IEnumerable<Client>> GetByClientNumberAsync(string clientNumber)
    {
        throw new NotImplementedException();
    }

    public async Task<Client> InsertAsync(Client client)
    {
        string query = "INSERT INTO Kunden (Kundennummer,Name,Telefonnummer,Email,user_id) VALUES (@Kundennummer,@Name,@Telefonnummer,@Email,@user_id)";
        Dictionary<string, object> queryParameters = new Dictionary<string, object>
        {
            { "Kundennummer", client.ClientNumber },
            { "Name", client.Name },
            { "Telefonnummer", client.Telefonnumber },
            { "Email", client.EmailAddress },
            { "@user_id", _userContext.CurrentUser.Id}
        };
        var newClient = await _connection.ExecuteScalarAsync<Client>(query, queryParameters);
        return newClient;
    }

    public async Task<bool> UpdateAsync(Client client)
    {
        bool succes = false;

        await _connection.ExecuteTransactionAsync(async (transaction) =>
        {
            string updateAddress = "update Adresse set Strasse = @Strasse, PLZ = @PLZ, Ort = @Ort where Kundennummer = @Kundennummer and user_id = @user_id";
            var addressParams = new Dictionary<string, object>
            {
                { "@Kundennummer", client.ClientNumber },
                { "@Strasse", client.Street },
                { "@PLZ", client.PLZ },
                { "@Ort", client.City },
                { "@user_id", _userContext.CurrentUser.Id}
            };
            await _connection.ExecuteNonQueryInTransactionAsync(transaction, updateAddress, addressParams);

            string updateClient = "update Kunden set Name = @Name, Telefonnummer = @Telefonnummer," +
            "Email = @Email where Kundennummer = @Kundennummer and user_id = @user_id";
            var updateParams = new Dictionary<string, object>
            {
                { "@Kundennummer", client.ClientNumber },
                { "@Name", client.Name },
                { "@Telefonnummer", client.Telefonnumber },
                { "@Email", client.EmailAddress },
                { "@user_id", _userContext.CurrentUser.Id}
            };
            int clientAffectedRows = await _connection.ExecuteNonQueryInTransactionAsync(transaction, updateClient, updateParams);
            succes = clientAffectedRows > 0;
        });

        return succes;
    }
}
