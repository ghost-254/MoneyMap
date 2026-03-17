using MoneyMap.Api.Models;

namespace MoneyMap.Api.DTOs.Transactions;

public sealed class TransactionResponseDto
{
    public int Id { get; init; }

    public int CategoryId { get; init; }

    public string CategoryName { get; init; } = string.Empty;

    public CategoryType CategoryType { get; init; }

    public decimal Amount { get; init; }

    public string? Description { get; init; }

    public DateTime OccurredOn { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime UpdatedAtUtc { get; init; }
}
