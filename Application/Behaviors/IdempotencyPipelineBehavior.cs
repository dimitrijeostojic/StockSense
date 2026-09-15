using Application.Abstractions.Idempotency;
using Application.Common.Errors;
using Application.OrderManagement.CreateOrder;
using Domain.Core;
using MediatR;

namespace Application.Behaviors;

internal sealed class IdempotencyPipelineBehavior<TRequest, TResponse>(
    IIdempotencyService idempotencyService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IdempotentRequest<TResponse>
    where TResponse : Result
{
    private readonly IIdempotencyService _idempotencyService = idempotencyService;

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        if (await _idempotencyService.RequestExistsAsync(request.RequestId, cancellationToken))
        {
            return (TResponse)(object)TResult<CreateOrderResponse>.Failure(ApplicationErrors.RequestAlreadyProccessed);
        }

        var response = await next();

        await _idempotencyService.CreateRequestAsync(request.RequestId, typeof(TRequest).Name, cancellationToken);

        return response;
    }
}
