using Application.Common.Collections;

namespace Application.CategoryManagement.GetAllCategories;

public sealed class GetAllCategoriesResponse(ICollection<GetAllCategoriesDto> items)
    : EntityCollectionResult<GetAllCategoriesDto>(items)
{

}