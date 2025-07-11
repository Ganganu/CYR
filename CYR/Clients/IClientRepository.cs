namespace CYR.Clients
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllAsync();
        Task<IEnumerable<Client>> GetByClientNumberAsync(string clientNumber);
        Task<Client> DeleteAsync(Client client);
        Task UpdateAsync(Client client);
        Task InsertAsync (Client client);
    }
}
