using CYR.OrderItems;

namespace CYR.Services;

public class JsonArticleImportMethod : IArticleImportMethod
{
    public string Method => "Json";

    public List<OrderItemCsvImport> Import(string fileName)
    {
        return [];
    }
}
