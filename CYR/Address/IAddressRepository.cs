using CYR.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
