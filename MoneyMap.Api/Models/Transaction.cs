namespace MoneyMap.Api.Models;

public sealed class Transaction
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public int CategoryId { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public DateTime OccurredOn { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;

    public Category Category { get; set; } = null!;
}
