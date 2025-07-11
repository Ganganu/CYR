using CYR.Core;
using System.Data.Common;

namespace CYR.Clients;

public class ClientRepository : IClientRepository
{
    private readonly IDatabaseConnection _connection;

    public ClientRepository(IDatabaseConnection connection)
    {
        _connection = connection;
    }

    public async Task<bool> DeleteAsync(Client client)
    {
        bool succes = false;

        await _connection.ExecuteTransactionAsync(async (transaction) =>
        {
            string addressQuery = "delete from Adresse where Kundennummer = @Kundennummer";
            var addressParams = new Dictionary<string, object>
            {
                { "@Kundennummer", client.ClientNumber }
            };
            await _connection.ExecuteNonQueryInTransactionAsync(transaction, addressQuery, addressParams);
            string orderQuery = "DELETE FROM Rechnungen WHERE Kundennummer = @Kundennummer";
            var orderParams = new Dictionary<string, object>
            {
                { "@Kundennummer", client.ClientNumber }
            };
            await _connection.ExecuteNonQueryInTransactionAsync(transaction, orderQuery, orderParams);
            string clientQuery = "DELETE FROM Kunden WHERE Kundennummer = @Kundennummer";
            var clientParams = new Dictionary<string, object>
            {
                { "@Kundennummer", client.ClientNumber }
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
        string query = "select Kunden.Kundennummer, Kunden.Name, Adresse.Strasse, Adresse.PLZ, Adresse.Ort, Kunden.Telefonnummer, Kunden.Email, Kunden.Erstellungsdatum\r\nfrom Kunden inner join Adresse on Kunden.Kundennummer = Adresse.Kundennummer";
        using (DbDataReader reader = (DbDataReader)await _connection.ExecuteSelectQueryAsync(query))
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
        string query = "INSERT INTO Kunden (Kundennummer,Name,Telefonnummer,Email,Erstellungsdatum) VALUES (@Kundennummer,@Name,@Telefonnummer,@Email,@Erstellungsdatum)";
        Dictionary<string,object> queryParameters = new Dictionary<string, object>
        {
            { "Kundennummer", client.ClientNumber },
            { "Name", client.Name },
            { "Telefonnummer", client.Telefonnumber },
            { "Email", client.EmailAddress },
            { "Erstellungsdatum", client.CreationDate }
        };
        var newClient = await _connection.ExecuteScalarAsync<Client>(query, queryParameters);
        return newClient;
    }

    public Task<bool> UpdateAsync(Client client)
    {
        throw new NotImplementedException();
    }
}
