namespace CYR.OrderItems;

public record OrderItemImport(int? ProductNumber = null, 
    string? Name = null, 
    string? Description = null, 
    double? Price = null, 
    string? ErrorText = null);

