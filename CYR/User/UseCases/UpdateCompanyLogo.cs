using CYR.Core;

namespace CYR.User.UseCases;

public class UpdateCompanyLogo
{
    private readonly IDatabaseConnection _databaseConnection;
    private readonly UserContext _userContext;

    public UpdateCompanyLogo(IDatabaseConnection databaseConnection, UserContext userContext)
    {
        _databaseConnection = databaseConnection;
        _userContext = userContext;
    }

    public async Task<int> UpdateCompanyLogoAsync(string? logo)
    {
        int affectedRows = 0;
        string query = "update company set logo = @logo";
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            {"logo", logo }
        };
        try
        {
          affectedRows = await _databaseConnection.ExecuteNonQueryAsync(query, parameters);  
        }
        catch (Exception)
        {
            throw;
        }
        return affectedRows;
    }

}
