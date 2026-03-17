using MoneyMap.Api.Models;

namespace MoneyMap.Api.DTOs.Categories;

public sealed class CategoryResponseDto
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public CategoryType Type { get; init; }

    public DateTime CreatedAtUtc { get; init; }
}
