using CYR.Core;

namespace CYR.Login;

public class LoginRepository
{
    private readonly IDatabaseConnection _databaseConnection;

    public LoginRepository(IDatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    // Existing login
    public async Task<bool> Login(string username, string password)
    {
        string query = @"
            SELECT COUNT(*) 
            FROM user 
            WHERE username = @username AND password = @password";

        var parameters = new Dictionary<string, object>
        {
            { "@username", username },
            { "@password", password } // This should be hashed ideally!
        };

        int result = await _databaseConnection.ExecuteScalarAsync<int>(query, parameters);
        return result > 0;
    }

    // New: Login with token
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

    // New: Store token after login
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

    // Optional: Clear token on logout
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
