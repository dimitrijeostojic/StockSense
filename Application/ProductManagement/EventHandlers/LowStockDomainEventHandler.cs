using Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.ProductManagement.EventHandlers;

internal sealed class LowStockDomainEventHandler(
    ILogger<LowStockDomainEvent> logger
    ) : INotificationHandler<LowStockDomainEvent>
{
    private readonly ILogger<LowStockDomainEvent> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Handle(LowStockDomainEvent notification, CancellationToken cancellationToken)
    {
        string message = $"{notification.ProductPublicId} - {notification.Quantity}";
        _logger.LogInformation(message);
    }
}
