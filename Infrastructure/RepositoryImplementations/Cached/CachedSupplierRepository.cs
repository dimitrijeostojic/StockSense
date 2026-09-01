using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Infrastructure.RepositoryImplementations.Cached;

public sealed class CachedSupplierRepository(
    ISupplierRepository decorated,
    IDistributedCache distributedCache) : ISupplierRepository
{
    private readonly ISupplierRepository _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
    private readonly IDistributedCache _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));

    public async Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        string key = $"supplier-{supplier.PublicId}-{supplier.TenantPublicId}";
        await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(supplier), CacheDefaults.DefaultOptions, cancellationToken);
        await _decorated.AddAsync(supplier, cancellationToken);
    }

    public async Task DeleteAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        string key = $"supplier-{supplier.PublicId}-{supplier.TenantPublicId}";
        await _distributedCache.RemoveAsync(key, cancellationToken);
        await _decorated.DeleteAsync(supplier, cancellationToken);
    }

    public async Task<(IEnumerable<Supplier> Items, int TotalCount)> GetAllAsync(Guid tenantPublicId, string? searchTerm = null, string? sortBy = null, bool isAscending = false, string? filterOn = null, string? filterQuery = null, int pageNumber = 1, int pageSize = 1000, CancellationToken cancellationToken = default)
    {
        return await _decorated.GetAllAsync(tenantPublicId, searchTerm, sortBy, isAscending, filterOn, filterQuery, pageNumber, pageSize, cancellationToken);
    }

    public async Task<Supplier?> GetByPublicIdAsync(Guid publicId, Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        string key = $"supplier-{publicId}-{tenantPublicId}";
        string? cached = await _distributedCache.GetStringAsync(key, cancellationToken);
        if (!string.IsNullOrEmpty(cached))
        {
            return JsonConvert.DeserializeObject<Supplier>(cached, new JsonSerializerSettings
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new PrivateResolver()
            });
        }
        var supplier = await _decorated.GetByPublicIdAsync(publicId, tenantPublicId, cancellationToken);
        if (supplier != null)
        {
            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(supplier), CacheDefaults.DefaultOptions, cancellationToken);
        }
        return supplier;
    }
}
