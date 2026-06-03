namespace Domain.Entities;

public sealed class Tenant : Entity
{
    public string Name { get; private set; } = null!;
    public string PIB { get; private set; } = null!;
    public string Address { get; private set; } = null!;
    public IReadOnlyCollection<ApplicationUser> ApplicationUsers => _applicationUsers.AsReadOnly();
    private readonly List<ApplicationUser> _applicationUsers = [];


    public static Tenant Create(string name, string pib, string address)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(pib);
        ArgumentException.ThrowIfNullOrWhiteSpace(address);
        return new Tenant
        {
            Name = name,
            PIB = pib,
            Address = address
        };
    }
}
