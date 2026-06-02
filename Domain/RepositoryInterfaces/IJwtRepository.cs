using Domain.Entities;

namespace Domain.RepositoryInterfaces;

public interface IJwtRepository
{
    Task<string> GenerateTokenAsync(ApplicationUser user, IEnumerable<string> roles);
}
