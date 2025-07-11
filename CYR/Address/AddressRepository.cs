using CYR.Core;

namespace CYR.Address
{
    internal class AddressRepository : IAddressRepository
    {
        private readonly IDatabaseConnection _databaseConnection;

        public AddressRepository(IDatabaseConnection databaseConnection)
        {
            this._databaseConnection = databaseConnection;
        }
        public Task DeleteAsync(AddressModel address)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AddressModel>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AddressModel>> GetByClientNumberAsync(string clientNumber)
        {
            throw new NotImplementedException();
        }

        public async Task InsertAsync(AddressModel address)
        {
            if (await CheckAddressExists(address)) return;
            string query = @"INSERT INTO Adresse (Kundennummer, Strasse, PLZ, Ort) 
                                VALUES (@Kundennummer, @Strasse, @PLZ, @Ort)";

            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            queryParameters["Kundennummer"] = address.CompanyName;
            queryParameters["Strasse"] = address.Street;
            queryParameters["PLZ"] = address.PLZ;
            queryParameters["Ort"] = address.City;
            int affectedRows = await _databaseConnection.ExecuteNonQueryAsync(query, queryParameters);
        }

        public Task UpdateAsync(AddressModel address)
        {
            throw new NotImplementedException();
        }
        private async Task<bool> CheckAddressExists(AddressModel address)
        {
            string query = @"
        SELECT COUNT(*) 
        FROM Adresse 
        WHERE Kundennummer = @Kundennummer 
          AND Strasse = @Strasse 
          AND PLZ = @PLZ 
          AND Ort = @Ort";

            Dictionary<string, object> queryParameters = new Dictionary<string, object>
        {
            { "@Kundennummer", address.CompanyName },
            { "@Strasse", address.Street },
            { "@PLZ", address.PLZ },
            { "@Ort", address.City }
        };

            int count = await _databaseConnection.ExecuteScalarAsync<int>(query, queryParameters);

            return count > 0;
        }
    }
}
