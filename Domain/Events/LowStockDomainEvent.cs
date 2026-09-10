using Domain.Primitives;

namespace Domain.Events;

public sealed record LowStockDomainEvent(Guid ProductPublicId, Guid TenantPublicId, int CurrentStock) : IDomainEvent;
