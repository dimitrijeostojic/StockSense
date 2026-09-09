using System.Diagnostics.CodeAnalysis;

namespace Domain.Core;

public class TResult<T> where T : class
{
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccess => Error == Error.None;
    public T? Value { get; }
    public Error? Error { get; } = Error.None;

    private TResult(T? value)
    {
        Value = value;
        Error = Error.None;
    }

    private TResult(Error error)
    {
        Value = default;
        Error = error;
    }

    public static TResult<T> Success(T value) => new(value);
    public static TResult<T> Failure(Error error) => new(error);
}
