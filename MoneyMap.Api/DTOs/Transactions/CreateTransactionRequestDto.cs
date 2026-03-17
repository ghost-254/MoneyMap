using System.ComponentModel.DataAnnotations;

namespace MoneyMap.Api.DTOs.Transactions;

public sealed class CreateTransactionRequestDto
{
    [Required]
    public int CategoryId { get; init; }

    [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
    public decimal Amount { get; init; }

    [MaxLength(250)]
    public string? Description { get; init; }

    [Required]
    public DateTime OccurredOn { get; init; }
}
