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

    public async Task<(IEnumerable<Supplier> Items, int TotalCount)> GetAllAsync(string? searchTerm, string? sortBy, bool isAscending, int pageNumber, int pageSize, Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        return await _decorated.GetAllAsync(searchTerm, sortBy, isAscending, pageNumber, pageSize, tenantPublicId, cancellationToken);
    }

    public async Task<Supplier?> GetByPublicIdAsync(Guid publicId, Guid tenantPublicId, CancellationToken cancellationToken = default)
    {
        string key = $"supplier-{publicId}-{tenantPublicId}";
        var cachedSupplier = await _distributedCache.GetStringAsync(key, cancellationToken);
        Supplier? supplier;
        if (string.IsNullOrEmpty(cachedSupplier))
        {
            supplier = await _decorated.GetByPublicIdAsync(publicId, tenantPublicId, cancellationToken);
            if (supplier == null)
            {
                return supplier;
            }
            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(supplier), CacheDefaults.DefaultOptions, cancellationToken);
            return supplier;
        }
        supplier = JsonConvert.DeserializeObject<Supplier>(cachedSupplier, new JsonSerializerSettings()
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new PrivateResolver()
        });
        return supplier;
    }
}
