using Domain.Core;
using MediatR;

namespace Application.AuthManagement.AcceptInvite;

public sealed record AcceptInviteRequest(
    string Email,
    string Token,
    string NewPassword) : IRequest<TResult<AcceptInviteResponse>>;
