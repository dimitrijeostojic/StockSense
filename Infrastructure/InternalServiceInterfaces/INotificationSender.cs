namespace Infrastructure.InternalServiceInterfaces;

public interface INotificationSender<TMessage>
{
    Task SendAsync(TMessage messageDto, CancellationToken cancellationToken);
}