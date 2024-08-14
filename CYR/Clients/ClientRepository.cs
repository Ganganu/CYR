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
            string query = "SELECT * FROM Kunden";
            using (DbDataReader reader = (DbDataReader)await _connection.ExecuteSelectQueryAsync(query))
            {
                while (await reader.ReadAsync())
                {
                    client = new Client
                    {
                        ClientNumber = reader["Kundennummer"].ToString(),
                        ClientName = reader["Name"].ToString(),
                        Address1 = reader["Adresse1"].ToString(),
                        Address2 = reader["Adresse2"].ToString(),
                        Address3 = reader["Adresse3"].ToString(),
                        Telefonnumber = reader["Telefonnummer"].ToString(),
                        Email = reader["Email"].ToString(),
                        CreationDate = Convert.ToDateTime(reader["Erstellungsdatum"])
                    };
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
