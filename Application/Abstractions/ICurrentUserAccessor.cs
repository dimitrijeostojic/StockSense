namespace Application.Abstractions;

public interface ICurrentUserAccessor
{
    string UserId { get; }
    string? Email { get; }
}
