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
            u.Logo = reader.IsDBNull(6) ? null : reader.GetString(6);
        }
        return u;
    }

    public async Task<int> InsertAsync(User model)
    {
        if (model is null) return 0;
        string? hashedPassword = _passwordHasherService.HashPassword(model.Password);
        int affectedRows = 0;
        if (model.Id is null)
        {
            string query = @"INSERT INTO user (username,password,role,user_logo) VALUES (@username,@password,@role,@user_logo)";
            Dictionary<string, object> queryParameters = new Dictionary<string, object>
            {
                { "username", model.Username},
                { "password", hashedPassword },
                { "role", model.Role },
                { "user_logo", model.Logo }
            };
            affectedRows = await _databaseConnection.ExecuteNonQueryAsync(query, queryParameters);
        }
        else
        {
            string updateUser = "update user  set username = @username, password = @password, role = @role, user_logo = @user_logo where username = @username";
            var updateuserParams = new Dictionary<string, object>
            {
                    { "username", model.Username },
                    { "password", model.Password },
                    { "role", model.Role },
                    { "user_logo", model.Logo }
            };
            affectedRows = await _databaseConnection.ExecuteNonQueryAsync(updateUser, updateuserParams);
        }
        return affectedRows;
    }    
}
