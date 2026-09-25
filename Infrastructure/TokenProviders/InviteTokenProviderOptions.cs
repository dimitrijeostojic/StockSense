using Microsoft.AspNetCore.Identity;

namespace Infrastructure.TokenProviders;

public sealed class InviteTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public InviteTokenProviderOptions()
    {
        TokenLifespan = TimeSpan.FromHours(72);
    }
}
