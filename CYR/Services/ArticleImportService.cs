using CYR.OrderItems;

namespace CYR.Services;

public sealed class ArticleImportService(IEnumerable<IArticleImportMethod> importMethods) : IArticleImportService
{
    public List<OrderItemCsvImport>? Import(string importMethod,string fileName)
    {
        var method = importMethods.FirstOrDefault(s => s.Method.Equals(importMethod,StringComparison.OrdinalIgnoreCase));
        return method is null ? null : method.Import(fileName);
    }
}
