using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public sealed class ApplicationUser : IdentityUser
{
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public int TenantId { get; private set; }
    public bool IsActive { get; private set; }
    public string? SeenTourPages { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Tenant? Tenant { get; private set; }

    private ApplicationUser()
    {

    }

    public static ApplicationUser Create(string userName, string email, string? firstName, string? lastName, int tenantId)
    {
        return new ApplicationUser
        {
            UserName = userName,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            TenantId = tenantId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Deactivate() => IsActive = false;

    public IReadOnlyList<string> GetSeenTourPages() =>
        SeenTourPages is null
            ? []
            : System.Text.Json.JsonSerializer.Deserialize<List<string>>(SeenTourPages) ?? [];

    public void CompleteTour(string pageName)
    {
        var pages = SeenTourPages is null
            ? []
            : System.Text.Json.JsonSerializer.Deserialize<List<string>>(SeenTourPages) ?? [];
        if (!pages.Contains(pageName))
        {
            pages.Add(pageName);
            SeenTourPages = System.Text.Json.JsonSerializer.Serialize(pages);
        }
    }

    public ApplicationUser WithFirstName(string? firstName)
    {
        FirstName = firstName;
        return this;
    }

    public ApplicationUser WithLastName(string? lastName)
    {
        LastName = lastName;
        return this;
    }
}
