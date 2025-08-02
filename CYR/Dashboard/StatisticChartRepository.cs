using CYR.Core;

namespace CYR.Dashboard;

public class StatisticChartRepository
{
    private readonly IDatabaseConnection _databaseConnection;
    public StatisticChartRepository(IDatabaseConnection databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<List<SalesPerMonth>> GetSalesPerMonth(int year)
    {
        List<SalesPerMonth> spm = new List<SalesPerMonth>();
        string query = @$"SELECT 
                        CAST(strftime('%m', Rechnungsdatum) AS INTEGER) AS Monat,
                        SUM(Bruttobetrag) AS Gesamtbetrag
                        FROM Rechnungen
                        WHERE Rechnungsdatum LIKE '{year}%'
                        GROUP BY Monat
                        ORDER BY Monat;";

        using var reader = await _databaseConnection.ExecuteReaderAsync(query, null);
        while (reader.Read())
        {            
            var month = reader.GetInt32(0);
            var amount = reader.GetDecimal(1);
            spm.Add(new SalesPerMonth(month, amount));   
        }
        return spm;
    }
}
public record SalesPerMonth(int Month, decimal Amount);