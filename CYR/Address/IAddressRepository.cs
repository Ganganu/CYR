namespace CYR.Address
{
    public interface IAddressRepository
    {
        Task<IEnumerable<AddressModel>> GetAllAsync();
        Task<IEnumerable<AddressModel>> GetByClientNumberAsync(string clientNumber);
        Task DeleteAsync(AddressModel client);
        Task UpdateAsync(AddressModel client);
        Task InsertAsync(AddressModel client);
    }
}
