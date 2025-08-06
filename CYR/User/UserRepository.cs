using System.Data.Common;
using CYR.Core;
using CYR.Services;
using OxyPlot;

namespace CYR.User;

public class UserRepository
{
    private readonly IDatabaseConnection _databaseConnection;
    private readonly IPasswordHasherService _passwordHasherService;
    public UserRepository(IDatabaseConnection databaseConnection, IPasswordHasherService passwordHasherService)
    {
        _databaseConnection = databaseConnection;
        _passwordHasherService = passwordHasherService;
    }

    public async Task<bool> DeleteAsync(User model)
    {
        return true;
    }

    public async Task<User> GetUserAsync(string username)
    {
        string query = "select * from user where username = @username";
        User u = new User();
        Dictionary<string, object> queryParameters = new Dictionary<string, object>
        {
            { "username", username }
        };
        using DbDataReader reader = (DbDataReader)await _databaseConnection.ExecuteSelectQueryAsync(query, queryParameters);
        while (await reader.ReadAsync())
        {
            u.Id = reader.GetInt16(0).ToString();
            u.Username = reader.GetString(1);
            u.Password = reader.GetString(2);
            u.Role = reader.GetString(3);
        }
        return u;
    }

    public async Task<int> InsertAsync(User model)
    {
        if (model is null) return 0;
        string? hashedPassword = _passwordHasherService.HashPassword(model.Password);

        string query = @"INSERT INTO user (username,password,role,user_logo) VALUES (@username,@password,@role,@user_logo)";
        Dictionary<string, object> queryParameters = new Dictionary<string, object>
        {
            { "username", model.Username},
            { "password", hashedPassword },
            { "role", model.Role },
            { "user_logo", model.Image }
        };
        int affectedRows = await _databaseConnection.ExecuteNonQueryAsync(query, queryParameters);
        return affectedRows;
    }

    public async Task<bool> UpdateUserAndCompanyInTransactionAsync(User userModel, Company companyModel)
    {
        if (userModel is null) return false;
        if (companyModel is null) return false;
        bool succes = false;
        await _databaseConnection.ExecuteTransactionAsync(async (transaction) =>
        {
            string updateUser = "update user  set username = @username, password = @password, role = @role, user_logo = @user_logo where username = @username";
            var updateuserParams = new Dictionary<string, object>
            {
                    { "username", userModel.Username },
                    { "password", userModel.Password },
                    { "role", userModel.Role },
                    { "user_logo", userModel.Image }
            };
            await _databaseConnection.ExecuteNonQueryInTransactionAsync(transaction, updateUser, updateuserParams);

            string updateCompany = @"UPDATE company SET name = @name,street = @street,city = @city,plz = @plz,house_number = @house_number,
                    telefon_number = @telefon_number,email_address = @email_address,bank_name = @bank_name,iban = @iban,
                    bic = @bic,ustidnr = @ustidnr,stnr = @stnr,logo = @logo
                    WHERE user_id = @user_id"; ;
            var updateCompanyParams = new Dictionary<string, object>
            {
                { "name", companyModel.Name },
                { "street", companyModel.Street },
                { "city", companyModel.City },
                { "plz", companyModel.PLZ },
                { "house_number", companyModel.HouseNumber },
                { "telefon_number", companyModel.TelefonNumber },
                { "email_address", companyModel.EmailAddress },
                { "bank_name", companyModel.BankName },
                { "iban", companyModel.IBAN },
                { "bic", companyModel.BIC },
                { "ustidnr", companyModel.USTIDNR },
                { "stnr", companyModel.STNR },
                { "logo", companyModel.Logo },
                { "user_id", userModel.Id }
            };
            int clientAffectedRows = await _databaseConnection.ExecuteNonQueryInTransactionAsync(transaction, updateCompany, updateCompanyParams);
            succes = clientAffectedRows > 0;
        });
        return succes;
    }    
}
