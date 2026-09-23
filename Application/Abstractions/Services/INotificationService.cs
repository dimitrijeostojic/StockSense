namespace Application.Abstractions.Services;

public interface INotificationService
{
    Task SendLowStockAlertAsync(string tenantPublicId, string productName, int currentStock, CancellationToken ct);
}
