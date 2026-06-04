using Domain.Abstractions;
using Domain.Entities;

namespace Domain.Events;

public sealed record OrderReceivedDomainEvent(Guid OrderPublicId, IReadOnlyCollection<OrderItem> OrderItems) : IDomainEvent;