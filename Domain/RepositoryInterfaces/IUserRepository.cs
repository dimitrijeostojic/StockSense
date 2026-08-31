using Domain.Entities;

namespace Domain.RepositoryInterfaces;

public interface IUserRepository
{
    void Delete(ApplicationUser user);
    Task<List<ApplicationUser>> GetAllUsersAsync(Guid tenantPublicId, CancellationToken cancellationToken);
    Task<ApplicationUser?> GetUserByPublicIdAsync(Guid userPublicId, Guid tenantPublicId, CancellationToken cancellationToken);
}
