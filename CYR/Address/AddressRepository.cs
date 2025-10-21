using CYR.Clients;
using CYR.Core;
using CYR.User;
using System.Data.Common;

namespace CYR.Address
{
    public class AddressRepository : IAddressRepository
    {
        private readonly IDatabaseConnection _databaseConnection;
        private readonly UserContext _userContext;

        public AddressRepository(IDatabaseConnection databaseConnection, UserContext userContext)
        {
            _databaseConnection = databaseConnection;
            _userContext = userContext;
        }
        public Task DeleteAsync(AddressModel address)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AddressModel>> GetAllAsync()
        {
            List<AddressModel> addresses = [];
            AddressModel address;
            string query = @"select Kundennummer, Strasse, PLZ, Ort, user_id from Adresse";
            using (DbDataReader reader = (DbDataReader)await _databaseConnection.ExecuteSelectQueryAsync(query))
            {
                while (await reader.ReadAsync())
                {
                    address = new()
                    {
                        CompanyName = reader.GetString(reader.GetOrdinal("Kundennummer")),
                        City = reader.GetString(reader.GetOrdinal("Ort")),
                        Street = reader.GetString(reader.GetOrdinal("Strasse")),
                        PLZ = reader.GetString(reader.GetOrdinal("PLZ"))
                    };
                    
                    addresses.Add(address);
                }
            }
            return addresses;
        }

        public Task<IEnumerable<AddressModel>> GetByClientNumberAsync(string clientNumber)
        {
            throw new NotImplementedException();
        }

        public async Task InsertAsync(AddressModel address)
        {
            if (await CheckAddressExists(address)) return;
            string query = @"INSERT INTO Adresse (Kundennummer, Strasse, PLZ, Ort, user_id) 
                                VALUES (@Kundennummer, @Strasse, @PLZ, @Ort, @user_id)";

            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            queryParameters["Kundennummer"] = address.CompanyName;
            queryParameters["Strasse"] = address.Street;
            queryParameters["PLZ"] = address.PLZ;
            queryParameters["Ort"] = address.City;
            queryParameters["user_id"] = _userContext.CurrentUser.Id;
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
          AND Ort = @Ort
          AND user_id = @user_id";

            Dictionary<string, object> queryParameters = new Dictionary<string, object>
        {
            { "@Kundennummer", address.CompanyName },
            { "@Strasse", address.Street },
            { "@PLZ", address.PLZ },
            { "@Ort", address.City },
            { "@user_id", _userContext.CurrentUser.Id }
        };

            int count = await _databaseConnection.ExecuteScalarAsync<int>(query, queryParameters);

            return count > 0;
        }
    }
}
