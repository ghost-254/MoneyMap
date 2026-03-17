using Microsoft.EntityFrameworkCore;
using MoneyMap.Api.Data;
using MoneyMap.Api.DTOs.Categories;
using MoneyMap.Api.Exceptions;
using MoneyMap.Api.Models;
using MoneyMap.Api.Services.Interfaces;

namespace MoneyMap.Api.Services;

public sealed class CategoryService(AppDbContext dbContext) : ICategoryService
{
    public async Task<IReadOnlyCollection<CategoryResponseDto>> GetCategoriesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Categories
            .AsNoTracking()
            .Where(category => category.UserId == userId)
            .OrderBy(category => category.Type)
            .ThenBy(category => category.Name)
            .Select(category => new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Type = category.Type,
                CreatedAtUtc = category.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoryResponseDto> CreateCategoryAsync(Guid userId, CreateCategoryRequestDto request, CancellationToken cancellationToken = default)
    {
        var categoryName = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            throw new BadRequestException("Category name is required.");
        }

        var exists = await dbContext.Categories
            .AsNoTracking()
            .AnyAsync(category => category.UserId == userId && category.Name == categoryName, cancellationToken);

        if (exists)
        {
            throw new ConflictException("A category with this name already exists.");
        }

        var category = new Category
        {
            UserId = userId,
            Name = categoryName,
            Type = request.Type,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
            Type = category.Type,
            CreatedAtUtc = category.CreatedAtUtc
        };
    }
}
