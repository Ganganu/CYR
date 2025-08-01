using CYR.Core;

namespace CYR.Dashboard;

public class StatisticOverviewRepository
{
    private readonly IDatabaseConnection _databaseConnection;
    public StatisticOverviewRepository(IDatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<int> GetNumberOfPaidInvoices()
    {
        string query = "select count(*) from Rechnungen where status = 1";
        int numberOfPaidInvoices = await _databaseConnection.ExecuteScalarAsync<int>(query);
        return numberOfPaidInvoices;
    }
    public async Task<int> GetNumberOfUnpaidInvoices()
    {
        string query = "select count(*) from Rechnungen where status = 0";
        int numberOfPaidInvoices = await _databaseConnection.ExecuteScalarAsync<int>(query);
        return numberOfPaidInvoices;
    }
    public async Task<decimal> GetSales(string year)
    {
        string query = @"select sum(Bruttobetrag) from Rechnungen where strftime('%Y', Rechnungsdatum) = '2025'";
        decimal sales = await _databaseConnection.ExecuteScalarAsync<decimal>(query);
        return sales;
    }
}
