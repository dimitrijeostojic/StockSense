using Domain.Dtos;
using Infrastructure.Data;
using Infrastructure.InternalServiceInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Infrastructure.BackgroundJobs;

public sealed class ProcessOutboxEmailMessagesBackgroundService(
    ILogger<ProcessOutboxEmailMessagesBackgroundService> logger,
    IServiceScopeFactory serviceScopeFactory
    ) : BackgroundService
{

    private readonly ILogger<ProcessOutboxEmailMessagesBackgroundService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        while (!stoppingToken.IsCancellationRequested)
        {

            //bacground service is singleton, applicationDbContext is scoped, so I have to go with this option
            using var scope = _serviceScopeFactory.CreateScope();
            var _applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var _emailSender = scope.ServiceProvider.GetRequiredService<INotificationSender<EmailMessageDto>>();

            var outboxEmailMessages = await _applicationDbContext.OutboxEmailMessages
            .Where(m => m.SentAtUtc == null && m.Attempts < 5)
            .Take(20)
            .ToListAsync(stoppingToken);

            AsyncRetryPolicy retryPolicy = Policy
           .Handle<Exception>()
           .WaitAndRetryAsync(
               retryCount: 10,
               sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
               onRetry: (exception, timespan, attempt, _) =>
               {
                   _logger.LogWarning(
                       "Send attempt {Attempt} failed. Waiting {Seconds}s. Error: {Error}",
                       attempt, timespan.TotalSeconds, exception.Message);
               });

            foreach (var item in outboxEmailMessages)
            {
                try
                {
                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        var message = new EmailMessageDto(item.To, item.Subject, item.Body);
                        await _emailSender.SendAsync(message, stoppingToken);
                        var updateMessage = item.WithSentAtUtc(DateTime.UtcNow);
                        _applicationDbContext.OutboxEmailMessages.Update(updateMessage);
                    });
                }
                catch (Exception ex)
                {
                    var failed = item.WithFailedAttempt(ex.Message);
                    _applicationDbContext.OutboxEmailMessages.Update(failed);
                    _logger.LogError(ex, "Failed to send email message with ID {MessageId}", item.Id);
                }
            }
            await _applicationDbContext.SaveChangesAsync(stoppingToken);
        }
    }
}
