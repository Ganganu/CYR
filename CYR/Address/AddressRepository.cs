using CYR.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CYR.Address
{
    internal class AddressRepository : IAddressRepository
    {
        private readonly IDatabaseConnection _databaseConnection;

        public AddressRepository(IDatabaseConnection databaseConnection) 
        {
            this._databaseConnection = databaseConnection;
        }
        public Task DeleteAsync(Address address)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Address>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Address>> GetByClientNumberAsync(string clientNumber)
        {
            throw new NotImplementedException();
        }

        public async Task InsertAsync(Address address)
        {
            string query = "INSERT INTO Adresse (Kundennummer,Strasse,PLZ,Ort) VALUES (@Kundennummer,@Strasse,@PLZ,@Ort)";
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            queryParameters["Kundennummer"] = address.CompanyName;
            queryParameters["Strasse"] = address.Street;
            queryParameters["PLZ"] = address.PLZ;
            queryParameters["Ort"] = address.City;

            await _databaseConnection.ExecuteSelectQueryAsync(query, queryParameters);
        }

        public Task UpdateAsync(Address address)
        {
            throw new NotImplementedException();
        }
    }
}
