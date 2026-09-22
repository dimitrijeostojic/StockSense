using Application.Abstractions.Services;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace Infrastructure.Interceptors;

internal sealed class AuditLogInterceptor(
    ICurrentUserAccessor currentUserAccessor) : SaveChangesInterceptor
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;

    public async override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {

        if (eventData.Context == null)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var entries = eventData.Context.ChangeTracker.Entries()
              .Where(e => e.Entity is not AuditLog
                  && e.State is EntityState.Added
                           or EntityState.Modified
                           or EntityState.Deleted).ToList();

        foreach (var entry in entries)
        {
            eventData.Context.Set<AuditLog>().Add(new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = _currentUserAccessor.UserId,
                UserEmail = _currentUserAccessor.Email,
                EntityId = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue?.ToString(),
                EntityName = entry.Entity.GetType().Name,
                Action = entry.State.ToString(),
                OldValues = entry.State != EntityState.Added
                    ? JsonSerializer.Serialize(entry.OriginalValues.ToObject())
                    : null,
                NewValues = entry.State != EntityState.Deleted
                    ? JsonSerializer.Serialize(entry.CurrentValues.ToObject())
                    : null,
                Timestamp = DateTime.UtcNow
            });
        }
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
