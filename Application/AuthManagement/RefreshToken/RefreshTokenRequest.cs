using Domain.Core;
using MediatR;

namespace Application.AuthManagement.RefreshToken;

public sealed record RefreshTokenRequest(string RefreshToken)
    : IRequest<TResult<RefreshTokenResponse>>;
