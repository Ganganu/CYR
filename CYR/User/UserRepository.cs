using System.Data.Common;
using CYR.Core;
using CYR.OrderItems;
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

    public async Task<bool> DeleteAsync(OrderItem orderItem)
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

        string query = @"INSERT INTO user (username,password,role) VALUES (@username,@password,@role)";
        Dictionary<string, object> queryParameters = new Dictionary<string, object>
        {
            { "username", model.Username},
            { "password", hashedPassword },
            { "role", model.Role }
        };
        int affectedRows = await _databaseConnection.ExecuteNonQueryAsync(query, queryParameters);
        return affectedRows;
    }

    public async Task<bool> UpdateAsync()
    {
        return true;
    }
}
