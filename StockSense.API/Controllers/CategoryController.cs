using Application.CategoryManagement.CreateCategory;
using Application.CategoryManagement.DeleteCategory;
using Application.CategoryManagement.GetAllCategories;
using Application.CategoryManagement.GetCategoryById;
using Application.CategoryManagement.UpdateCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockSense.API.Extensions;

namespace StockSense.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CategoryController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpPost]
    public async Task<IActionResult> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{publicId}")]
    public async Task<IActionResult> GetCategoryByIdAsync([FromRoute] Guid publicId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCategoryByIdRequest(publicId), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("{publicId}")]
    public async Task<IActionResult> UpdateCategoryAsync([FromRoute] Guid publicId, [FromBody] UpdateCategoryRequestBody request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateCategoryRequest(publicId, request.Name, request.Description), cancellationToken);
        return result.ToActionResult();
    }

    [HttpDelete("{publicId}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] Guid publicId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteCategoryRequest(publicId), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategoriesAsync(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllCategoriesRequest(), cancellationToken);
        return result.ToActionResult();
    }
}