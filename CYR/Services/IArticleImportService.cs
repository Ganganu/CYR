using CYR.OrderItems;

namespace CYR.Services;

public interface IArticleImportService
{
    public List<OrderItemCsvImport> Import();
}
