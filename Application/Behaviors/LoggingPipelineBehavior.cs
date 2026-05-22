using Domain.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Behaviors;

public sealed class LoggingPipelineBehavior<TRequest, TResponse>(
     ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger
    )
    : IPipelineBehavior<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
        where TResponse : Result<TResponse>
{
    private readonly ILogger<LoggingPipelineBehavior<TRequest, TResponse>> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        _logger.LogInformation("Starting request {@RequestName}, {@DateTimeUtc}", typeof(TRequest).Name, DateTime.UtcNow);

        var result = await next();

        if (!result.IsSuccess)
        {
            _logger.LogInformation("Request failure {@RequestName}, {@Error}, {@DateTimeUtc}", typeof(TRequest).Name, result.Error, DateTime.UtcNow);
        }

        _logger.LogInformation("Completed request {@RequestName}, {@DateTimeUtc}", typeof(TRequest).Name, DateTime.UtcNow);

        return result;
    }
}
