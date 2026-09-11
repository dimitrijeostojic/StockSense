using System.Diagnostics.CodeAnalysis;

namespace Domain.Core;

public class TResult<T> : Result where T : class
{
    [MemberNotNullWhen(true, nameof(Value))]
    public new bool IsSuccess => base.IsSuccess;

    public T? Value { get; }

    private TResult(T? value) : base()
    {
        Value = value;
    }

    private TResult(Error error) : base(error)
    {
        Value = default;
    }

    public static TResult<T> Success(T value) => new(value);
    public static new TResult<T> Failure(Error error) => new(error);
}
