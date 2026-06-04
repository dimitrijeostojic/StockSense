using Domain.Entities;

namespace Domain.RepositoryInterfaces;

public interface ITenantRepository
{
    Task AddAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task<Tenant?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tenant>> GetAllAsync(CancellationToken cancellationToken = default);
    void Delete(Tenant tenant);
    Task<Tenant?> GetByPIBAsync(string PIB, CancellationToken cancellationToken = default);
}
