using System.Security.Cryptography;

namespace Domain.Entities;

public sealed class RefreshToken : Entity
{
    public string? Token { get; private set; }
    public string? UserId { get; private set; }
    public DateTime ExpiresOnUtc { get; private set; }
    public bool IsRevoked { get; private set; }
    public ApplicationUser? User { get; private set; }

    private RefreshToken()
    {

    }

    public static RefreshToken Create(string userId)
    {
        return new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = userId,
            ExpiresOnUtc = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };
    }

    public RefreshToken Revoke()
    {
        IsRevoked = true;
        return this;
    }
}
