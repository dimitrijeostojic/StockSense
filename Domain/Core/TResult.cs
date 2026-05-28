namespace Domain.Core;

public class TResult<T> : IResult where T : class
{
    public bool IsSuccess => Error == Error.None;
    public T? Value { get; }
    public Error? Error { get; }

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

    public static TResult<T> Success(T? value) => new(value);
    public static TResult<T> Failure(Error error) => new(error);
}
