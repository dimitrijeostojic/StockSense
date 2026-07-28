using Domain.Primitives;

namespace Domain.Events;

public sealed record LowStockDomainEvent(Guid ProductPublicId, int Quantity) : IDomainEvent;
