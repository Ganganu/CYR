using CYR.Core;

namespace CYR.User.UseCases;

public class UpdateCompanyLogo(IDatabaseConnection databaseConnection, UserContext userContext)
{

    public async Task<int> UpdateCompanyLogoAsync(string? logo)
    {
        int affectedRows = 0;
        string query = "update company set logo = @logo where user_id = @user_id";
        Dictionary<string, object> parameters = new()
        {
            {"logo", logo },
            {"user_id", userContext.CurrentUser.Id}
        };
        try
        {
          affectedRows = await databaseConnection.ExecuteNonQueryAsync(query, parameters);  
        }
        catch (Exception)
        {
            throw;
        }
        return affectedRows;
    }

}
