namespace CYR.OrderItems;

public record OrderItemCsvImport(int? ProductNumber = null, 
    string? Name = null, 
    string? Description = null, 
    double? Price = null, 
    string? ErrorText = null);

