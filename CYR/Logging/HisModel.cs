namespace CYR.Logging;

public class HisModel
{
    public int Id { get; set; }
    public LoggingType LoggingType { get; set; }
    public string? UserId { get; set; }
    public int AddressId { get; set; }
    public string? ClientId { get; set; }
    public int? InvoiceId { get; set; }
    public string OrderItemId { get; set; }
    public int UnitOfMeasureId { get; set; }
    public string? Message { get; set; }
    public DateTime Timestamp { get; set; }
}
