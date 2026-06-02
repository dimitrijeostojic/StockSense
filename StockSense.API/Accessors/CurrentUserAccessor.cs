using Application.Abstractions;
using System.Security.Claims;

namespace StockSense.API.Accessors;

public sealed class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    public string UserId => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    public string? Email => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
    public Guid TenantPublicId => Guid.Parse(_httpContextAccessor.HttpContext?.User.FindFirstValue("tenant_public_id") ?? Guid.Empty.ToString());
}
