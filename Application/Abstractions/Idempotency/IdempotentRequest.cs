using MediatR;

namespace Application.Abstractions.Idempotency;

public abstract record IdempotentRequest<TResponse>(Guid RequestId) : IRequest<TResponse>;
