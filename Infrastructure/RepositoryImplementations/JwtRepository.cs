using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.RepositoryImplementations;

public sealed class JwtRepository(IOptions<JwtOptions> options) : IJwtRepository
{
    private readonly JwtOptions _options = options.Value;

    public async Task<string> GenerateTokenAsync(ApplicationUser user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id),
            new (JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new (JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new ("tenant_public_id", user.TenantId.ToString())
        };

        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)), SecurityAlgorithms.HmacSha256);

        var tokenHandler = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            null,
            DateTime.UtcNow.AddMinutes(_options.ExpirationInMinutes),
            signingCredentials
            );

        var token = new JwtSecurityTokenHandler().WriteToken(tokenHandler);
        return token;
    }
}