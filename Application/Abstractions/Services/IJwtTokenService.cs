using Domain.Entities;

namespace Application.Abstractions.Services;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user, Guid tenantPublicId, IEnumerable<string> roles);
}
