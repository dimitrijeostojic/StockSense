using Domain.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.TokenProviders;

public sealed class InviteTokenProvider : DataProtectorTokenProvider<ApplicationUser>
{
    public InviteTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<InviteTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<ApplicationUser>> logger)
        : base(dataProtectionProvider, options, logger)
    {
    }
}
