namespace CYR.Clients
{
    public class RetrieveClients : IRetrieveClients
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
