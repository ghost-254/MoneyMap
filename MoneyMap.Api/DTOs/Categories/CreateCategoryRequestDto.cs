using System.ComponentModel.DataAnnotations;
using MoneyMap.Api.Models;

namespace MoneyMap.Api.DTOs.Categories;

public sealed class CreateCategoryRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; init; } = string.Empty;

    [Required]
    public CategoryType Type { get; init; }
}
