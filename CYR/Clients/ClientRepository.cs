using CYR.Core;
using CYR.OrderItems;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYR.Clients
{
    public class ClientRepository : IClientRepository
    {
        private readonly IDatabaseConnection _connection;

        public ClientRepository(IDatabaseConnection connection)
        {
            _connection = connection;
        }

        public async Task<Client> DeleteAsync(Client client)
        {
            string query = "delete from Kunden where Kundennummer = @Kundennummer";
            Dictionary<string, object> queryParameters = new Dictionary<string, object>
            {
                { "Kundennummer", client.ClientNumber}
            };
            int affectedRows = await _connection.ExecuteNonQueryAsync(query, queryParameters);
            if (affectedRows <= 0)
                return null;
            return client;
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

        public async Task InsertAsync(Client client)
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
            int affectedRows = await _connection.ExecuteNonQueryAsync(query, queryParameters);
        }

        public Task UpdateAsync(Client client)
        {
            throw new NotImplementedException();
        }
    }
}
