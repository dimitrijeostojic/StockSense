using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.RepositoryImplementations.Cached;

public static class CacheDefaults
{
    public static readonly DistributedCacheEntryOptions DefaultOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };
}