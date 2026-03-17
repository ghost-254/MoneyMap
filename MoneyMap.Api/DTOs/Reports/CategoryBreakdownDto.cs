using MoneyMap.Api.Models;

namespace MoneyMap.Api.DTOs.Reports;

public sealed class CategoryBreakdownDto
{
    public int CategoryId { get; init; }

    public string CategoryName { get; init; } = string.Empty;

    public CategoryType Type { get; init; }

    public decimal TotalAmount { get; init; }
}
