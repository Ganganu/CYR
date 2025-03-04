namespace CYR.Invoice.Model
{
    public class InvoicePositionModel
    {
        public string Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
