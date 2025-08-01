using CYR.Core;

namespace CYR.Dashboard;

public class StatisticChartRepository
{
    private readonly IDatabaseConnection _databaseConnection;
    public StatisticChartRepository(IDatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    //public async Task<SalesPerMonth> GetSalesPerMonth()
    //{
    //    string query = @$"select k.Name, sum(r.Bruttobetrag) as amount from Kunden k inner join Rechnungen r
    //                      on k.Kundennummer = r.Kundennummer
    //                      group by r.Kundennummer
    //                      order by sum(r.Bruttobetrag) desc 
    //                      limit 1";

    //    using var reader = await _databaseConnection.ExecuteReaderAsync(query, null);
    //}
}
public record SalesPerMonth(string Month, decimal Amount);