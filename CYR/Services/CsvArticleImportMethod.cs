using CYR.OrderItems;
using System.Globalization;
using System.IO;

namespace CYR.Services;

public class CsvArticleImportMethod : IArticleImportMethod
{
    public string Method => "Csv";

    public List<OrderItemImport> Import(string fileName)
    {
        List<OrderItemImport> data = [];
        try
        {
            data = File.ReadLines(fileName)
                   .Skip(0)
                   .Select(ParseLineToModel)
                   .Where(item => item != null)
                   .ToList();
        }
        catch (System.IO.IOException ioException)
        {
            data.Add(new OrderItemImport(ErrorText: $"Die Datei ist möglicherweise bereits geöffnet. \n \n {ioException}"));
        }
        catch (Exception ex)
        {
            data.Add(new OrderItemImport(ErrorText: $"Unbekannter Fehler. \n \n {ex}"));
        }
        return data;
    }

    private OrderItemImport ParseLineToModel(string line)
    {
        var columns = line.Split(';');
        if (columns.Length < 4) return null;

        static string Clean(string input) => input.Replace("\"", "").Trim();

        string productNumber = Clean(columns[0]);
        string articleName = Clean(columns[1]);
        string artcielDescription = Clean(columns[2]);
        string articlePrice = Clean(columns[3]);

        IFormatProvider culture = articlePrice.Contains(',')
        ? CultureInfo.GetCultureInfo("de-DE")
        : CultureInfo.InvariantCulture;

        int parsedProductNumber = int.TryParse(productNumber, out int pn) ? pn : 0;
        double parsedArticlePrice = double.TryParse(articlePrice, NumberStyles.Any, culture, out double pAP) ? pAP : 0.0D;

        return new OrderItemImport(parsedProductNumber, articleName, artcielDescription, parsedArticlePrice);
    }
}
