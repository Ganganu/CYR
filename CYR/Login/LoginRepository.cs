using CYR.Core;
using CYR.Services;

namespace CYR.Login;

public class LoginRepository
{
    private readonly IDatabaseConnection _databaseConnection;
    private readonly IPasswordHasherService _passwordHasherService;

    public LoginRepository(IDatabaseConnection databaseConnection, IPasswordHasherService passwordHasherService)
    {
        _databaseConnection = databaseConnection;
        _passwordHasherService = passwordHasherService;
    }

    public async Task<bool> Login(string username, string password)
    {
        string query = @"SELECT password FROM user WHERE username = @username";
        var parameters = new Dictionary<string, object>
        {
            { "@username", username }
        };

        string? storedHash = await _databaseConnection.ExecuteScalarAsync<string>(query, parameters);

        if (string.IsNullOrEmpty(storedHash))
        {
            return false;
        }

        return _passwordHasherService.VerifyPassword(storedHash, password);
    }

    public async Task<bool> LoginWithToken(string username, string token)
    {
        string query = @"
            SELECT COUNT(*) 
            FROM user 
            WHERE username = @username AND token = @token";

        var parameters = new Dictionary<string, object>
        {
            { "@username", username },
            { "@token", token }
        };

        int result = await _databaseConnection.ExecuteScalarAsync<int>(query, parameters);
        return result > 0;
    }

    public async Task RememberTokenAsync(string username, string token)
    {
        string query = @"
            UPDATE user 
            SET token = @token 
            WHERE username = @username";

        var parameters = new Dictionary<string, object>
        {
            { "@username", username },
            { "@token", token }
        };

        await _databaseConnection.ExecuteNonQueryAsync(query, parameters);
    }

    public async Task LogoutAsync(string username)
    {
        string query = @"
            UPDATE user 
            SET token = NULL 
            WHERE username = @username";

        var parameters = new Dictionary<string, object>
        {
            { "@username", username }
        };

        await _databaseConnection.ExecuteNonQueryAsync(query, parameters);
    }
}
