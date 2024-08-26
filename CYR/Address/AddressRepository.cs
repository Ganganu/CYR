using CYR.Clients;
using CYR.Core;
using System.Data.Common;
using System.Net;

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
            string query = @"
                            IF NOT EXISTS (
                                SELECT 1 FROM Adresse 
                                WHERE Kundennummer = @Kundennummer 
                                AND Strasse = @Strasse 
                                AND PLZ = @PLZ 
                                AND Ort = @Ort
                            )
                            BEGIN
                                INSERT INTO Adresse (Kundennummer, Strasse, PLZ, Ort) 
                                VALUES (@Kundennummer, @Strasse, @PLZ, @Ort)
                            END";
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            queryParameters["Kundennummer"] = address.CompanyName;
            queryParameters["Strasse"] = address.Street;
            queryParameters["PLZ"] = address.PLZ;
            queryParameters["Ort"] = address.City;
            int affectedRows = await _databaseConnection.ExecuteNonQueryAsync(query, queryParameters);
        }

        public Task UpdateAsync(Address address)
        {
            throw new NotImplementedException();
        }
    }
}
