namespace Domain.Core;

public class Result
{
    public bool IsSuccess { get; }

    public Error Error { get; }

    public Result()
    {
        IsSuccess = true;
        Error = Error.None;
    }

    public Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }


    public static Result Success() => new();
    public static Result Failure(Error error) => new(error);
}
