namespace Domain.Entities;

public sealed class IdempotentRequest
{
    public Guid RequestId { get; private set; }
    public string Name { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }

    public IdempotentRequest(Guid requestId, string name, DateTime createdOnUtc)
    {
        RequestId = requestId;
        Name = name;
        CreatedOnUtc = createdOnUtc;
    }

    public static IdempotentRequest Create(Guid requestId, string name, DateTime createdOnUtc)
    {
        return new IdempotentRequest(requestId, name, createdOnUtc);
    }
}
