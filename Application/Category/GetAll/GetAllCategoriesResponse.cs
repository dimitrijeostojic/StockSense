using Application.Common.Extensions;

namespace Application.Category.GetAll;

public sealed class GetAllCategoriesResponse(ICollection<GetAllCategoriesDto> items)
    : EntityCollectionResult<GetAllCategoriesDto>(items)
{

}