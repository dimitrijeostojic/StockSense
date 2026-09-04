namespace Application.UserManagement.GetMyUser;

public sealed class GetMyUserResponse
{
    public IReadOnlyCollection<string> Roles { get; set; } = [];
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
}
