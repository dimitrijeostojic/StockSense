using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Infrastructure.RepositoryImplementations.Cached;

public sealed class CachedCategoryRepository(
    ICategoryRepository categoryRepository,
    IDistributedCache distributedCache) : ICategoryRepository
{
    private readonly ICategoryRepository _decorated = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    private readonly IDistributedCache _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        string key = $"category-{category.PublicId}-{category.TenantPublicId}";
        await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(category), CacheDefaults.DefaultOptions, cancellationToken);
        await _decorated.AddAsync(category, cancellationToken);
        await _distributedCache.RemoveAsync($"categories-{category.TenantPublicId}", cancellationToken);
    }

    public async Task DeleteAsync(Category category, CancellationToken cancellationToken = default)
    {
        string key = $"category-{category.PublicId}-{category.TenantPublicId}";
        await _distributedCache.RemoveAsync(key, cancellationToken);
        await _decorated.DeleteAsync(category, cancellationToken);
        await _distributedCache.RemoveAsync($"categories-{category.TenantPublicId}", cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetAllAsync(Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        string key = $"categories-{tenantPublicId}";
        var cachedCategories = await _distributedCache.GetStringAsync(key, cancellationToken);
        IEnumerable<Category> categories = [];
        if (string.IsNullOrEmpty(cachedCategories))
        {
            categories = await _decorated.GetAllAsync(tenantPublicId, cancellationToken);
            if (!categories.Any())
            {
                return categories;
            }
            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(categories), CacheDefaults.DefaultOptions, cancellationToken);
            return categories;
        }
        categories = JsonConvert.DeserializeObject<IEnumerable<Category>>(cachedCategories, new JsonSerializerSettings()
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new PrivateResolver()
        })!;

        return categories;
    }

    public Task<Category?> GetByPublicIdAsync(Guid publicId, Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        return _decorated.GetByPublicIdAsync(publicId, tenantPublicId, cancellationToken);
    }
}
