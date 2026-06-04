using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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

    public async Task<Tenant?> GetByIdAsync(int tenantId, CancellationToken cancellationToken)
    {
        return await _authDbContext.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);
    }

    public async Task<Tenant?> GetByPIBAsync(string PIB, CancellationToken cancellationToken = default)
    {
        return await _authDbContext.Tenants.FirstOrDefaultAsync(t => t.PIB == PIB, cancellationToken);
    }

    public async Task<Tenant?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default)
    {
        return await _authDbContext.Tenants.Include(t => t.ApplicationUsers).FirstOrDefaultAsync(t => t.PublicId == publicId, cancellationToken);
    }
}
