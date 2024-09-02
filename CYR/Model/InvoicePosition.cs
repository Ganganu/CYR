namespace CYR.Model
{
    public class InvoicePosition
    {
        public string? Id { get; set; }
        public string? InvoiceNumber { get; set; }
        public OrderItem.OrderItem? OrderItem { get; set; }
        public int Quantity { get; set; }
        public string? UnitOfMeasure { get; set; }
        public decimal TotalPrice => OrderItem?.Price * Quantity ?? 0;
    }
}
