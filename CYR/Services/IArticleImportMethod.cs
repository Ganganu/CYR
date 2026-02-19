using CYR.OrderItems;

namespace CYR.Services;

public interface IArticleImportMethod
{
    string Method { get; }
    List<OrderItemImport> Import(string fileName);

}
