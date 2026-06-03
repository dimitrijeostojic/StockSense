using Application.Common.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Data.Interceptors;

public sealed class UpdateAuditableEntitiesInterceptor(ICurrentUserAccessor currentUserAccessor)
    : SaveChangesInterceptor
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor ?? throw new ArgumentNullException(nameof(currentUserAccessor));

    public override ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        if (dbContext == null)
        {
            return base.SavedChangesAsync(eventData, result, cancellationToken);
        }
        var entries = dbContext.ChangeTracker.Entries<AuditableEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Property(e => e.ModifiedAt).CurrentValue = DateTime.UtcNow;
                entry.Property(e => e.ModifiedBy).CurrentValue = _currentUserAccessor.UserId;
            }
            if (entry.State == EntityState.Added)
            {
                entry.Property(e => e.CreatedAt).CurrentValue = DateTime.UtcNow;
                entry.Property(e => e.CreatedBy).CurrentValue = _currentUserAccessor.UserId;
            }
        }
        return new ValueTask<int>(result);
    }
}
