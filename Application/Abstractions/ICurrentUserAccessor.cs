namespace Application.Abstractions;

public interface ICurrentUserAccessor
{
    string UserId { get; }
    Guid TenantPublicId { get; }
    string? Email { get; }
}
