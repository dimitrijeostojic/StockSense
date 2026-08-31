using Domain.Core;
using MediatR;

namespace Application.UserManagement.Delete;

public sealed class DeleteUserRequest : IRequest<Result>
{
    public Guid UserPublicId { get; set; }
}
