using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImplementations;

public sealed class UserRepository(AuthDbContext authDbContext) : IUserRepository
{
    private readonly AuthDbContext _authDbContext = authDbContext ?? throw new ArgumentNullException(nameof(authDbContext));

    public void Delete(ApplicationUser user)
    {
        _authDbContext.Users.Remove(user);
    }

    public async Task<List<ApplicationUser>> GetAllUsersAsync(Guid tenantPublicId, CancellationToken cancellationToken)
    {
        var tenant = await _authDbContext.Tenants.FirstOrDefaultAsync(t => t.PublicId == tenantPublicId, cancellationToken: cancellationToken);
        return tenant == null
            ? []
            : await _authDbContext.Users.Where(u => u.TenantId == tenant.Id).ToListAsync(cancellationToken);
    }

    public async Task<ApplicationUser?> GetUserByPublicIdAsync(Guid userPublicId, Guid tenantPublicId, CancellationToken cancellationToken)
    {
        var tenant = await _authDbContext.Tenants.FirstOrDefaultAsync(t => t.PublicId == tenantPublicId, cancellationToken);
        if (tenant is null)
        {
            return null;
        }

        return await _authDbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userPublicId.ToString() && u.TenantId == tenant.Id, cancellationToken);
    }
}
