namespace CYR.Dashboard;

public sealed record ActivityModel(DateTime? Timestamp, string? ActivityType, string? Title, string? Description, string? EntityId,
    string? ClientName, string? Amount, string? Status);
