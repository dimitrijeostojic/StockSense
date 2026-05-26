namespace Domain.Core;

public interface IResult
{
    bool IsSuccess { get; }
    Error? Error { get; }
}
