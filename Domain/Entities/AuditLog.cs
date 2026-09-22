namespace Domain.Entities;

public class AuditLog
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string? UserId { get; init; }
    public string? UserEmail { get; init; }
    public string? EntityId { get; init; }
    public string? EntityName { get; init; }
    public string? Action { get; init; }  // Added/Modified/Deleted
    public string? OldValues { get; init; }
    public string? NewValues { get; init; }
    public DateTime Timestamp { get; init; }
}