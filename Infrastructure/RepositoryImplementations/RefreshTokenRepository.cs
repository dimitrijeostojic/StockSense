using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImplementations;

public sealed class RefreshTokenRepository(AuthDbContext authDbContext) : IRefreshTokenRepository
{
    private readonly AuthDbContext _authDbContext = authDbContext ?? throw new ArgumentNullException(nameof(authDbContext));

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        await _authDbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken)
    {
        return await _authDbContext.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }
}
