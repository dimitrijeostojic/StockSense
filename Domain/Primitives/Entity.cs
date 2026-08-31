namespace Domain.Primitives;

public abstract class Entity
{
    public int Id { get; set; }
    public Guid PublicId { get; private set; } = Guid.NewGuid();
}
