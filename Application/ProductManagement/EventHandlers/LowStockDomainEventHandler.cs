using Application.Abstractions.Services;
using Application.Emails;
using Domain.Events;
using Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.ProductManagement.EventHandlers;

internal sealed class LowStockDomainEventHandler(
    ILogger<LowStockDomainEvent> logger,
    IEmailService emailService,
    ITenantRepository tenantRepository,
    IProductRepository productRepository,
    INotificationService notificationService
    ) : INotificationHandler<LowStockDomainEvent>
{
    private const string _message = "Low stock triggered for Product {ProductId} in Tenant {TenantId}, current stock: {CurrentStock}";
    private readonly ILogger<LowStockDomainEvent> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    private readonly ITenantRepository _tenantRepository = tenantRepository ?? throw new ArgumentNullException(nameof(tenantRepository));
    private readonly IProductRepository _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    private readonly INotificationService _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));

    public async Task Handle(LowStockDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(_message, notification.ProductPublicId, notification.TenantPublicId, notification.CurrentStock);

        var tenant = await _tenantRepository.GetByPublicIdAsync(notification.TenantPublicId, cancellationToken);
        if (tenant is null || tenant.ApplicationUsers.Count == 0)
        {
            return;
        }
        var product = await _productRepository.GetByPublicIdAsync(notification.ProductPublicId, notification.TenantPublicId, cancellationToken);
        if (product is null)
        {
            _logger.LogWarning("Product {ProductPublicId} not found for low stock notification", notification.ProductPublicId);
            return;
        }

        foreach (var user in tenant.ApplicationUsers)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                continue;
            }

            var emailMessage = EmailTemplates.LowStockAlert(
                user.Email,
                user.FirstName ?? user.UserName ?? "there",
                product.Name,
                notification.CurrentStock,
                product.MinimumStockQuantity,
                product.Supplier!.Name);

            await _emailService.SendAsync(emailMessage, cancellationToken);
        }

        await _notificationService.SendLowStockAlertAsync(notification.TenantPublicId.ToString(), product.Name, notification.CurrentStock, cancellationToken);
    }
}
