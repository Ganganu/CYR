using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYR.Clients
{
    public class RetrieveClients
    {
        private readonly IClientRepository _clientRepository;

        public RetrieveClients(IClientRepository clientRepository) 
        {
            this._clientRepository = clientRepository;
        }
        public async Task<Client> Handle()
        {
            IEnumerable<Client> clients = await _clientRepository.GetAllAsync();
            return (Client)clients;
        }
    }
}
