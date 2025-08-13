namespace CYR.Logging;

public enum LoggingType
{
    UserCreated = 1000,
    UserDeleted = 1001,
    UserUpdated = 1002,
    UserLogin = 1003,
    UserLogout = 1004,

    AddressCreated = 2000,
    AddressDeleted = 2001,
    AddressUpdated = 2002,

    ClientCreated = 3000,
    ClientDeleted = 3001,
    ClientUpdated = 3002,

    InvoiceCreated = 4000,
    InvoiceDeleted = 4001,
    InvoiceUpdated = 4002,

    OrderItemCreated = 5000,
    OrderItemDeleted = 5001,
    OrderItemUpdated = 5002,

    UnitOfMeasureCreated = 6000,
    UnitOfMeasureDeleted = 6001,
    UnitOfMeasureUpdated = 6002
}
