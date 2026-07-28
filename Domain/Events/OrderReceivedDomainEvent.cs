using Domain.Entities;
using Domain.Primitives;

namespace Domain.Events;

public sealed record OrderReceivedDomainEvent(Guid OrderPublicId, IReadOnlyCollection<OrderItem> OrderItems) : IDomainEvent;