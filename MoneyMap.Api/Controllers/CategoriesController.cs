using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMap.Api.DTOs.Categories;
using MoneyMap.Api.Extensions;
using MoneyMap.Api.Services.Interfaces;

namespace MoneyMap.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/categories")]
public sealed class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CategoryResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<CategoryResponseDto>>> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await categoryService.GetCategoriesAsync(User.GetUserId(), cancellationToken);
        return Ok(categories);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoryResponseDto>> CreateCategory(CreateCategoryRequestDto request, CancellationToken cancellationToken)
    {
        var category = await categoryService.CreateCategoryAsync(User.GetUserId(), request, cancellationToken);
        return Created($"/api/categories/{category.Id}", category);
    }
}
