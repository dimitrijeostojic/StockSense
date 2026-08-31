using Domain.Core;
using MediatR;

namespace Application.UserManagement.GetAll;

public sealed class GetAllUsersRequest : IRequest<TResult<GetAllUsersResponse>>;