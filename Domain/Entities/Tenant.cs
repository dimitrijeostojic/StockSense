using Domain.Primitives;

namespace Domain.Entities;

public sealed class Tenant : Entity
{
    public string Name { get; private set; }
    public string Pib { get; private set; }
    public string? Address { get; private set; }
    public string? LogoUrl { get; private set; }

    public IReadOnlyCollection<ApplicationUser> ApplicationUsers => _applicationUsers.AsReadOnly();
    private readonly List<ApplicationUser> _applicationUsers = [];

    private Tenant(string name, string pib, string? address = null, string? logoUrl = null)
    {
        Name = name;
        Pib = pib;
        Address = address;
        LogoUrl = logoUrl;
    }

    public static Tenant Create(string name, string pib, string? address = null, string? logoUrl = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(pib);
        return new Tenant(name, pib, address, logoUrl);
    }

    public Tenant WithAddress(string? address)
    {
        Address = address;
        return this;
    }

    public Tenant WithLogoUrl(string? logoUrl)
    {
        LogoUrl = logoUrl;
        return this;
    }
}
