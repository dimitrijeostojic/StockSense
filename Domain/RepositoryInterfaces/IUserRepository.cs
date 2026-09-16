using Domain.Entities;

namespace Domain.RepositoryInterfaces;

public interface IUserRepository
{
    Task<List<ApplicationUser>> GetAllUsersAsync(Guid tenantPublicId, CancellationToken cancellationToken);
    Task<ApplicationUser?> GetUserByPublicIdAsync(Guid userPublicId, Guid tenantPublicId, CancellationToken cancellationToken);
}
