using MoneyMap.Api.DTOs.Categories;

namespace MoneyMap.Api.Services.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyCollection<CategoryResponseDto>> GetCategoriesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<CategoryResponseDto> CreateCategoryAsync(Guid userId, CreateCategoryRequestDto request, CancellationToken cancellationToken = default);
}
