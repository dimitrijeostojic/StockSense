namespace Domain.Primitives;

public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;

    public DateTime? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }
}
