using Domain.Entities;

namespace Application.Abstractions.Services;

public interface IJwtTokenService
{
    string GenerateTokenAsync(ApplicationUser user, IEnumerable<string> roles);
}
