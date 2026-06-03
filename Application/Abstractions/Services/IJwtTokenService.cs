using Domain.Entities;

namespace Application.Abstractions.Services;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user, IEnumerable<string> roles);
}
