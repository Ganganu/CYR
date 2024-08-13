using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYR.Clients
{
    public class ClientRepository : IClientRepository
    {
        public ClientRepository() 
        {

        }
        public Task DeleteAsync(Client client)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Client>> GetAllAsync()
        {
            throw new NotImplementedException();
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
