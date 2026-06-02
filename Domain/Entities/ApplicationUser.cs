using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public sealed class ApplicationUser : IdentityUser
{
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public int TenantId { get; private set; }
    public Tenant? Tenant { get; private set; }

    public static ApplicationUser Create(string userName, string email, string? firstName, string? lastName, int tenantId)
    {
        return new ApplicationUser
        {
            UserName = userName,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            TenantId = tenantId
        };
    }
}
