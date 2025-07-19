
namespace CYR.OrderItems;

public interface IPrintOrderItemService
{
    void Print(IEnumerable<OrderItem> orderItems);
}