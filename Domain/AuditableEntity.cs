namespace Domain;

public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAt { get; private set; }
    public string? CreatedBy { get; private set; }

    public DateTime? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }
}
