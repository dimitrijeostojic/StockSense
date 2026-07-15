using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Infrastructure.RepositoryImplementations;

public sealed class CategoryRepository(
    ApplicationDbContext dbContext,
    IDistributedCache distributedCache)
    : ICategoryRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private readonly IDistributedCache _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        string key = $"category-{category.PublicId}-{category.TenantPublicId}";
        await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(category), cancellationToken);
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

    public async Task<Category?> GetByPublicIdAsync(Guid publicId, Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        string key = $"category-{publicId}-{tenantPublicId}";
        string? cachedCategory = await _distributedCache.GetStringAsync(key, token: cancellationToken);
        Category? category;
        if (string.IsNullOrEmpty(cachedCategory))
        {
            category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.PublicId == publicId && c.TenantPublicId == tenantPublicId, cancellationToken);
            if (category == null)
            {
                return null;
            }
            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(category), token: cancellationToken);
            return category;
        }
        category = JsonConvert.DeserializeObject<Category>(cachedCategory, new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new PrivateResolver()
        });

        if (category is not null)
        {
            _dbContext.Set<Category>().Attach(category);
        }
        return category;
    }
}


public class PrivateResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);
        if (!jsonProperty.Writable)
        {
            var property = member as PropertyInfo;
            bool hasPrivateSetter = property?.GetSetMethod(true) != null;
            jsonProperty.Writable = hasPrivateSetter;
        }
        return jsonProperty;
    }

}