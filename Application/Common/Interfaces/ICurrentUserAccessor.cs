namespace Application.Common.Interfaces;

public interface ICurrentUserAccessor
{
    string UserId { get; }
    Guid TenantPublicId { get; }
    string? Email { get; }
}
