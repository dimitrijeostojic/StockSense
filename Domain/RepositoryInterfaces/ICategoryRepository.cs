using Domain.Entities;

namespace Domain.RepositoryInterfaces;

public interface ICategoryRepository
{
    Task AddAsync(Category category, CancellationToken cancellationToken = default);
    Task<Category?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    void Delete(Category category);
}
