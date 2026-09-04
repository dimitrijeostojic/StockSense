using Domain.Core;
using MediatR;

namespace Application.UserManagement.GetMyUser;

public sealed class GetMyUserRequest : IRequest<TResult<GetMyUserResponse>>
{
}
