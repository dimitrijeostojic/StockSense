namespace Domain.Core;

public class Result<T> : IResult where T : class
{
    public bool IsSuccess => Error == Error.None;
    public T? Value { get; }
    public Error? Error { get; }

    private Result(T? value)
    {
        Value = value;
        Error = Error.None;
    }

    private Result(Error error)
    {
        Value = default;
        Error = error;
    }

    public static Result<T> Success(T? value) => new(value);
    public static Result<T> Failure(Error error) => new(error);
}
