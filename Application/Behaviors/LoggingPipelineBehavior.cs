using Domain.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Behaviors;

public sealed class LoggingPipelineBehavior<TRequest, TResponse>(
    ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
{
    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        logger.LogInformation(
            "Starting request {@RequestName} {@DateTimeUtc}",
            typeof(TRequest).Name,
            DateTime.UtcNow);

        var result = await next();

        if (!result.IsSuccess)
        {
            logger.LogWarning(
                "Request failure {@RequestName} {@Error} {@DateTimeUtc}",
                typeof(TRequest).Name,
                result.Error,
                DateTime.UtcNow);
        }
        else
        {
            logger.LogInformation(
                "Completed request {@RequestName} {@DateTimeUtc}",
                typeof(TRequest).Name,
                DateTime.UtcNow);
        }

        return result;
    }
}
