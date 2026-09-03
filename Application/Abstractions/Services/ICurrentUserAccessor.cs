namespace Application.Abstractions.Services;

public interface ICurrentUserAccessor
{
    string UserId { get; }
    Guid TenantPublicId { get; }
    string? Email { get; }
}
