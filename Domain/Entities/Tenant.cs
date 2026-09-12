using Domain.Primitives;

namespace Domain.Entities;

public sealed class Tenant : Entity
{
    public string Name { get; private set; }
    public string Pib { get; private set; }
    public string? Address { get; private set; }
    public byte[]? Logo { get; private set; }

    public IReadOnlyCollection<ApplicationUser> ApplicationUsers => _applicationUsers.AsReadOnly();
    private readonly List<ApplicationUser> _applicationUsers = [];

    private Tenant(string name, string pib, string? address = null, byte[]? logo = null)
    {
        Name = name;
        Pib = pib;
        Address = address;
        Logo = logo;
    }

    public static Tenant Create(string name, string pib, string? address = null, byte[]? logo = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(pib);
        return new Tenant(name, pib, address, logo);
    }

    public Tenant WithName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
        return this;
    }

    public Tenant WithAddress(string? address)
    {
        Address = address;
        return this;
    }

    public Tenant WithLogo(byte[]? logo)
    {
        Logo = logo;
        return this;
    }
}
