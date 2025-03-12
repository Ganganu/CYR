namespace CYR.Address
{
    public interface IAddressRepository
    {
        Task<IEnumerable<Address>> GetAllAsync();
        Task<IEnumerable<Address>> GetByClientNumberAsync(string clientNumber);
        Task DeleteAsync(Address client);
        Task UpdateAsync(Address client);
        Task InsertAsync(Address client);
    }
}
