using CYR.OrderItems;

namespace CYR.Services;

public interface IArticleImportService
{
    public List<OrderItemImport> Import(string importMethod, string fileName);
}
