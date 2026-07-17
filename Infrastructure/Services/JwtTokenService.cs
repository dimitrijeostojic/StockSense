using Application.Abstractions.Services;
using Domain.Entities;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services;

public sealed class JwtTokenService(IOptions<JwtOptions> options) : IJwtTokenService
{
    private readonly JwtOptions _options = options.Value;

    public string GenerateToken(ApplicationUser user, Guid tenantPublicId, string tenantName, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id),
            new (JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new (JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new ("tenant_public_id", tenantPublicId.ToString()),
            new ("tenant_name", tenantName)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

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