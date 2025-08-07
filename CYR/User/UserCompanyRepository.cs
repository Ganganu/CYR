using System.Data.Common;
using CYR.Core;

namespace CYR.User;

public class UserCompanyRepository
{
    private readonly IDatabaseConnection _databaseConnection;
    public UserCompanyRepository(IDatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<UserCompany?> GetAsync(int id)
    {
        UserCompany? userCompany = null;

        string query = $@"select u.username, u.role, u.created_at, u.user_logo, c.id, c.name, c.street, c.city, c.plz, c.house_number,
                      c.telefon_number, c.email_address, c.bank_name, c.iban, c.bic, c.ustidnr, c.stnr, c.logo, c.created_at
                      from user u inner join company c
                      on u.id = c.user_id
                      where u.id = {id}";

        using (DbDataReader reader = (DbDataReader)await _databaseConnection.ExecuteSelectQueryAsync(query))
        {
            if (reader != null && await reader.ReadAsync())
            {
                string? username = reader.IsDBNull(0) ? null : reader.GetString(0);
                string? role = reader.IsDBNull(1) ? null : reader.GetString(1);
                string? createdAtText = reader.IsDBNull(2) ? null : reader.GetString(2);
                string? userLogo = reader.IsDBNull(3) ? null : reader.GetString(3);
                int companyId = 0;
                if (!reader.IsDBNull(4) && int.TryParse(reader.GetValue(4).ToString(), out var idParsed))
                    companyId = idParsed;
                string? companyName = reader.IsDBNull(5) ? null : reader.GetString(5);
                string? companyStreet = reader.IsDBNull(6) ? null : reader.GetString(6);
                string? companyCity = reader.IsDBNull(7) ? null : reader.GetString(7);
                string? companyPlz = reader.IsDBNull(8) ? null : reader.GetString(8);
                string? companyHouseNumber = reader.IsDBNull(9) ? null : reader.GetString(9);
                string? companyTelefonNumber = reader.IsDBNull(10) ? null : reader.GetString(10);
                string? companyEmailAddress = reader.IsDBNull(11) ? null : reader.GetString(11);
                string? companyBankName = reader.IsDBNull(12) ? null : reader.GetString(12);
                string? companyIban = reader.IsDBNull(13) ? null : reader.GetString(13);
                string? companyBic = reader.IsDBNull(14) ? null : reader.GetString(14);
                string? companyUstidnr = reader.IsDBNull(15) ? null : reader.GetString(15);
                string? companyStnr = reader.IsDBNull(16) ? null : reader.GetString(16);
                string? companyLogo = reader.IsDBNull(17) ? null : reader.GetString(17);
                string? companyCreatedAtText = reader.IsDBNull(18) ? null : reader.GetString(18);

                // Parse DateTime safely
                DateTime? createdAt = DateTime.TryParse(createdAtText, out var parsedCreatedAt) ? parsedCreatedAt : null;

                userCompany = new UserCompany(
                    username, role, createdAt, userLogo, companyId, companyName, companyStreet, companyCity,
                    companyPlz, companyHouseNumber, companyTelefonNumber, companyEmailAddress,
                    companyBankName, companyIban, companyBic, companyUstidnr, companyStnr, companyLogo
                );
            }
        }
        return userCompany;
    }
}
