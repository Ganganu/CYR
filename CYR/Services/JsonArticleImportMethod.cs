using CYR.OrderItems;
using System.IO;
using System.Text.Json;

namespace CYR.Services;

public class JsonArticleImportMethod : IArticleImportMethod
{
    public string Method => "Json";

    public List<OrderItemCsvImport> Import(string fileName)
    {
        string jsonContent = File.ReadAllText(fileName);
        var data = JsonSerializer.Deserialize<List<OrderItemCsvImport>>(jsonContent);
        return data ?? [];
    }
}
