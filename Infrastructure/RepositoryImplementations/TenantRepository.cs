using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.Data;

namespace Infrastructure.RepositoryImplementations;

public sealed class TenantRepository(AuthDbContext authDbContext) : ITenantRepository
{
    private readonly AuthDbContext _authDbContext = authDbContext ?? throw new ArgumentNullException(nameof(authDbContext));

    public async Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        await _authDbContext.Tenants.AddAsync(tenant, cancellationToken);
    }

    public void Delete(Tenant tenant)
    {
        _authDbContext.Tenants.Remove(tenant);
    }

    public Task<IEnumerable<Tenant>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Tenant?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
