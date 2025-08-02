using System.Data.Common;
using CYR.Core;
using CYR.OrderItems;

namespace CYR.User;

public class UserRepository
{
    private readonly IDatabaseConnection _databaseConnection;
    public UserRepository(IDatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<bool> DeleteAsync(OrderItem orderItem)
    {
        return true;
    }

    public async Task<User> GetUserAsync(string username, string password)
    {
        string query = "select * from user where username = @username and password = @password";
        User u = new User();
        Dictionary<string, object> queryParameters = new Dictionary<string, object>
        {
            { "username", username },
            { "password", password}
        };
        using DbDataReader reader = (DbDataReader)await _databaseConnection.ExecuteSelectQueryAsync(query, queryParameters);
        while (await reader.ReadAsync())
        {
            u.Id = reader.GetInt16(0).ToString();
            u.Username = reader.GetString(1);
            u.Password = reader.GetString(2);
            u.Role = reader.GetString(3);
            u.CompanyId = reader.GetInt16(4).ToString();
        }
        return u;
    }

    public async Task InsertAsync()
    {
        
    }

    public async Task<bool> UpdateAsync()
    {
        return true;
    }
}
