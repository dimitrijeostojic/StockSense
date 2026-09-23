using Application.Abstractions.Services;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services;

public sealed class NotificationService(IHubContext<NotificationHub> hubContext) : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));

    public async Task SendLowStockAlertAsync(string tenantPublicId, string productName, int currentStock, CancellationToken ct)
    {
        await _hubContext.Clients.Group($"tenant-{tenantPublicId}")
            .SendAsync("LowStockAlert", new { productName, currentStock }, ct);
    }
}
