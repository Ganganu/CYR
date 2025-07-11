namespace CYR.Clients
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllAsync();
        Task<IEnumerable<Client>> GetByClientNumberAsync(string clientNumber);
        Task<bool> DeleteAsync(Client client);
        Task<bool> UpdateAsync(Client client);
        Task<Client> InsertAsync (Client client);
        
    }
}
