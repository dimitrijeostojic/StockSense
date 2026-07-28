using Domain.Primitives;

namespace Domain.Events;

public sealed record OrderReceivedDomainEvent(Guid OrderPublicId, Guid TenantPublicId, IReadOnlyCollection<OrderReceivedItem> OrderItems) : IDomainEvent;