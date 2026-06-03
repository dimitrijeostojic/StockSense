using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImplementations;

public sealed class CategoryRepository(ApplicationDbContext dbContext)
    : ICategoryRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        await _dbContext.Categories.AddAsync(category, cancellationToken);
    }

    public void Delete(Category category)
    {
        _dbContext.Categories.Remove(category);
    }

    public async Task<IEnumerable<Category>> GetAllAsync(Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories.Where(c => c.TenantPublicId == tenantPublicId).ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetByPublicIdAsync(Guid publicId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories.FirstOrDefaultAsync(c => c.PublicId == publicId, cancellationToken);
    }
}
