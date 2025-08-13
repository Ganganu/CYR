namespace CYR.Logging;

public class HisModel
{
    public int Id { get; set; }
    public LoggingType LoggingType { get; set; }
    public int UserId { get; set; }
    public int AddressId { get; set; }
    public int ClientId { get; set; }
    public int InvoiceId { get; set; }
    public int OrderItemId { get; set; }
    public int UnitOfMeasureId { get; set; }
    public string? Message { get; set; }
    public DateTime Timestamp { get; set; }
}
