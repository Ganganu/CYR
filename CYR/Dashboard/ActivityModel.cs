namespace CYR.Dashboard;

public sealed record ActivityModel(string? ClientName, string? ClientId, string? InvoiceAmount, DateTime? InvoiceDate, string? Message);
