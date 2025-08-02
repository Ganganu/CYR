using CYR.Core;
using CYR.User;

namespace CYR.Dashboard;

public class StatisticOverviewRepository
{
    private readonly IDatabaseConnection _databaseConnection;
    private readonly UserContext _userContext;
    public StatisticOverviewRepository(IDatabaseConnection databaseConnection, UserContext userContext)
    {
        _databaseConnection = databaseConnection;
        _userContext = userContext;
    }

    public async Task<int> GetNumberOfPaidInvoices()
    {
        string query = @$"select count(*) from Rechnungen where status = 1 and user_id = {_userContext.CurrentUser.Id}";
        int numberOfPaidInvoices = await _databaseConnection.ExecuteScalarAsync<int>(query);
        return numberOfPaidInvoices;
    }
    public async Task<int> GetNumberOfUnpaidInvoices()
    {
        string query = @$"select count(*) from Rechnungen where status = 0 and user_id = {_userContext.CurrentUser.Id}";
        int numberOfPaidInvoices = await _databaseConnection.ExecuteScalarAsync<int>(query);
        return numberOfPaidInvoices;
    }
    public async Task<decimal> GetSales(string year)
    {
        string query = @$"select sum(Bruttobetrag) from Rechnungen where strftime('%Y', Rechnungsdatum) = '{year}' and user_id = {_userContext.CurrentUser.Id}";
        decimal sales = await _databaseConnection.ExecuteScalarAsync<decimal>(query);
        return sales;
    }
    public async Task<decimal> GetSalesActualMonth()
    {
        string currentYear = DateTime.Now.ToString("yyyy");
        string currentMonth = DateTime.Now.ToString("MM");
        string query = $@"select sum(Bruttobetrag) 
                      from Rechnungen 
                      where strftime('%Y', Rechnungsdatum) = '{currentYear}' 
                      and strftime('%m', Rechnungsdatum) = '{currentMonth}'
                      and user_id = {_userContext.CurrentUser.Id}";
        decimal sales = await _databaseConnection.ExecuteScalarAsync<decimal>(query);
        return sales;
    }
    public async Task<int> GetInvoicesActualMonth()
    {
        string currentYear = DateTime.Now.ToString("yyyy");
        string currentMonth = DateTime.Now.ToString("MM");
        string query = $@"select count(*) 
                      from Rechnungen 
                      where strftime('%Y', Rechnungsdatum) = '{currentYear}' 
                      and strftime('%m', Rechnungsdatum) = '{currentMonth}'
                      and user_id = {_userContext.CurrentUser.Id}";
        int invoices = await _databaseConnection.ExecuteScalarAsync<int>(query);
        return invoices;
    }
    public async Task<ClientAndSales> GetClientsAndSales()
    {
        string query = @$"select k.Name, sum(r.Bruttobetrag) as amount from Kunden k inner join Rechnungen r
                          on k.Kundennummer = r.Kundennummer where k.user_id = {_userContext.CurrentUser.Id}
                          group by r.Kundennummer
                          order by sum(r.Bruttobetrag) desc 
                          limit 1";

        using var reader = await _databaseConnection.ExecuteReaderAsync(query, null);
        while (reader.Read())
        {
            var name = reader.GetString(0);
            var amount = reader.GetDecimal(1);
            return new ClientAndSales(name, amount);
        }
        return new ClientAndSales("keine Daten", 0);
    }
}
public record ClientAndSales(string Client, decimal Amount);
