namespace CYR.OrderItems;

public static class OrderItemMapper
{
    public static OrderItem ToOrderItem(this OrderItemImport importItem)
    {
        return new OrderItem
        {
            Name = importItem.Name,
            Description = importItem.Description,
            Price = importItem.Price,
        };
    }
}
