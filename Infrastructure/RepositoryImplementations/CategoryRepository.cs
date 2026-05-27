using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.Data;

namespace Infrastructure.RepositoryImplementations;

public sealed class CategoryRepository(ApplicationDbContext dbContext)
    : ICategoryRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        await _dbContext.Categories.AddAsync(category, cancellationToken);
    }

    public void Delete(Guid publicId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
