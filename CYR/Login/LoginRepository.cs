using CYR.Core;

namespace CYR.Login;

public class LoginRepository
{
    private readonly IDatabaseConnection _databaseConnection;
    public LoginRepository(IDatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<bool> Login(string username, string password)
    {
        bool succes = false;
        string query = $@"select count(*) from user where username = @username and password = @password";
        Dictionary<string,object> parameters = new Dictionary<string, object>
        {
            { "@username", username },
            { "@password", password }
        };
        int numberOfPaidInvoices = await _databaseConnection.ExecuteScalarAsync<int>(query,parameters);
        succes = numberOfPaidInvoices > 0;
        return succes;
    }
}
