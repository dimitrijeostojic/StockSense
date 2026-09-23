using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Hubs;

[Authorize]
public sealed class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var tenantPublicId = Context.User?.FindFirst("tenant_public_id")?.Value;
        if (tenantPublicId is not null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant-{tenantPublicId}");
        }
        await Clients.Client(Context.ConnectionId).SendAsync($"Thank you for connecting {Context.User?.Identity?.Name}");
        await base.OnConnectedAsync();
    }
}
