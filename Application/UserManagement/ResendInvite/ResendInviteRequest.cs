using Domain.Core;
using MediatR;

namespace Application.UserManagement.ResendInvite;

public sealed record ResendInviteRequest(Guid UserPublicId) : IRequest<Result>;
