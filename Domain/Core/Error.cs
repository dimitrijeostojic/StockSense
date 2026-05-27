namespace Domain.Core;

public sealed record Error(string Code, ErrorType Type, string? Description = null)
{
    public static readonly Error None = new(string.Empty, ErrorType.None, null);
}
