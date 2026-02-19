using CYR.OrderItems;
using System.IO;
using System.Text.Json;

namespace CYR.Services;

public class JsonArticleImportMethod : IArticleImportMethod
{
    public string Method => "Json";

    public List<OrderItemImport> Import(string fileName)
    {
        string jsonContent = File.ReadAllText(fileName);
        List<OrderItemImport>? data = [];
        try
        {
            data = JsonSerializer.Deserialize<List<OrderItemImport>>(jsonContent);
        }
        catch (JsonException jsonEx)
        {
            data!.Add(new OrderItemImport(ErrorText: $"Json kann nicht konvertiert werden! \n {jsonEx}"));
        }
        catch (Exception ex)
        {
            data!.Add(new OrderItemImport(ErrorText: $"Etwas ist schiefgelaufen \n {ex}"));
        }
        return data ?? [];
    }
}
