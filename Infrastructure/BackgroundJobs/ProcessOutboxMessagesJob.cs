using Domain.Entities;
using Domain.Primitives;
using Infrastructure.Data;
using MassTransit.Mediator;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;

namespace Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
internal class ProcessOutboxMessagesJob(
    ApplicationDbContext dbContext,
    IMediator mediator) : IJob
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    public async Task Execute(IJobExecutionContext context)
    {
        var messages = await _dbContext.Set<OutboxMessage>()
            .Where(m => m.ProccessedOnUtc == null)
            .Take(20)
            .ToListAsync(context.CancellationToken);

        foreach (var message in messages)
        {
            var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(message.Content);
            if (domainEvent is null)
            {
                continue;
            }
            await _mediator.Publish(message, context.CancellationToken);

            message.ProccessedOnUtc = DateTime.UtcNow;
        }
        await _dbContext.SaveChangesAsync();
    }
}
