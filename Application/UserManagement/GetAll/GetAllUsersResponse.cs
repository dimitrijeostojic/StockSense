using Application.Common.Collections;

namespace Application.UserManagement.GetAll;

public sealed class GetAllUsersResponse(
    IEnumerable<GetAllUsersDto> items)
    : EntityCollectionResult<GetAllUsersDto>(items)
{
}
