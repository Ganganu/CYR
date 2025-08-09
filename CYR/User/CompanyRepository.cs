using System.Data.Common;
using CYR.Core;

namespace CYR.User;

public class CompanyRepository
{
    private readonly IDatabaseConnection _databaseConnection;
    private readonly UserContext _userContext;
    public CompanyRepository(IDatabaseConnection connection, UserContext userContext)
    {
        _databaseConnection = connection;
        _userContext = userContext;
    }

    public async Task<Company> GetCompanyAsync(int id)
    {
        string query = "select * from company where user_id = @user_id";
        Company u = new Company();
        Dictionary<string, object> queryParameters = new Dictionary<string, object>
        {
            { "user_id", id }
        };
        using DbDataReader reader = (DbDataReader)await _databaseConnection.ExecuteSelectQueryAsync(query, queryParameters);
        while (await reader.ReadAsync())
        {
            u.Id = reader.GetInt16(0).ToString();
            u.Name = reader.GetString(1);
            u.Street = reader.GetString(2);
            u.City = reader.GetString(3);
            u.Plz = reader.GetString(4);
            u.HouseNumber = reader.GetString(5);
            u.TelefonNumber = reader.GetString(6);
            u.EmailAddress = reader.GetString(7);
            u.BankName = reader.GetString(8);
            u.Iban = reader.GetString(9);
            u.Bic = reader.GetString(10);
            u.Ustidnr = reader.GetString(11);
            u.Stnr = reader.GetString(12);
            u.Logo = reader.IsDBNull(13) ? null : reader.GetString(13);
        }
        return u;
    }

    public async Task<int> InsertAsync(Company model)
    {
        if (model is null) return 0;

        int affectedRows = 0;
        if (model.Id is null)
        {
            string query = @"INSERT INTO company (name,street,city,plz,house_number,telefon_number,email_address,bank_name,
                        iban,bic,ustidnr,stnr,logo,user_id) VALUES (@name,@street,@city,@plz,@house_number,@telefon_number,@email_address,@bank_name,
                        @iban,@bic,@ustidnr,@stnr,@logo,@user_id)";
            Dictionary<string, object> queryParameters = new Dictionary<string, object>
            {
                    { "name", model.Name },
                    { "street", model.Street },
                    { "city", model.City },
                    { "plz", model.Plz },
                    { "house_number", model.HouseNumber },
                    { "telefon_number", model.TelefonNumber },
                    { "email_address", model.EmailAddress },
                    { "bank_name", model.BankName },
                    { "iban", model.Iban },
                    { "bic", model.Bic },
                    { "ustidnr", model.Ustidnr },
                    { "stnr", model.Stnr },
                    { "logo", model.Logo },
                    { "user_id", _userContext.CurrentUser.Id}
            };
            try
            {
                affectedRows = await _databaseConnection.ExecuteNonQueryAsync(query, queryParameters);

            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        else
        {
            string query = @"UPDATE company SET name = @name, street = @street, city = @city, plz = @plz, house_number = @house_number,
                            telefon_number = @telefon_number, email_address = @email_address,bank_name = @bank_name, iban = @iban,
                            bic = @bic, ustidnr = @ustidnr, stnr = @stnr, logo = @logo, user_id = @user_id where id = @id";
            Dictionary<string, object> queryParameters = new Dictionary<string, object>
            {
                    { "name", model.Name },
                    { "street", model.Street },
                    { "city", model.City },
                    { "plz", model.Plz },
                    { "house_number", model.HouseNumber },
                    { "telefon_number", model.TelefonNumber },
                    { "email_address", model.EmailAddress },
                    { "bank_name", model.BankName },
                    { "iban", model.Iban },
                    { "bic", model.Bic },
                    { "ustidnr", model.Ustidnr },
                    { "stnr", model.Stnr },
                    { "logo", model.Logo },
                    { "user_id", _userContext.CurrentUser.Id},
                    { "id", model.Id},
            };
            try
            {
                affectedRows = await _databaseConnection.ExecuteNonQueryAsync(query, queryParameters);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        return affectedRows;
    }
}
