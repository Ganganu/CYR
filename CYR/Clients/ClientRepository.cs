using CYR.Core;
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
            this._connection = connection;
        }

        public Task DeleteAsync(Client client)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            List<Client> clientList = new List<Client>();
            Client client;
            string query = "select Kunden.Kundennummer, Kunden.Name, Adresse.Strasse, Kunden.Telefonnummer, Kunden.Email, Kunden.Erstellungsdatum\r\nfrom Kunden inner join Adresse on Kunden.Kundennummer = Adresse.Kundennummer";
            using (DbDataReader reader = (DbDataReader)await _connection.ExecuteSelectQueryAsync(query))
            {
                while (await reader.ReadAsync())
                {
                    client = new Client();
                    client.ClientNumber = reader["Kundennummer"].ToString();
                    client.ClientName = reader["Name"].ToString();
                    client.Address = reader["Strasse"].ToString();
                    client.Telefonnumber = reader["Telefonnummer"].ToString();
                    client.Email = reader["Email"].ToString();
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

        public Task InsertAsync(Client client)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Client client)
        {
            throw new NotImplementedException();
        }
    }
}
