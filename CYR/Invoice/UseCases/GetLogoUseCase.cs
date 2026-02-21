using CYR.Clients;
using CYR.Core;
using CYR.Invoice.InvoiceModels;
using CYR.User;
using System.Data.Common;

namespace CYR.Invoice.UseCases;

public class GetLogoUseCase
{
    private readonly IDatabaseConnection _databaseConnection;
    private readonly UserContext _userContext;

    public GetLogoUseCase(IDatabaseConnection databaseConnection, UserContext userContext)
    {
        _databaseConnection = databaseConnection;
        _userContext = userContext;
    }

    public async Task<string?> GetLogoAsync()
    {
        string? logo = string.Empty;
        string? query = "select logo from company where user_id = @user_id";
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            {"user_id", _userContext.CurrentUser.Id}
        };
        using (DbDataReader reader = (DbDataReader)await _databaseConnection.ExecuteSelectQueryAsync(query, parameters))
        {
            if (await reader.ReadAsync())
            {
                logo = reader["logo"].ToString();               
            }
        }
        return logo;
    }
}
