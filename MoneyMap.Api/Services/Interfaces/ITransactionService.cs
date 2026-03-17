using MoneyMap.Api.DTOs.Transactions;

namespace MoneyMap.Api.Services.Interfaces;

public interface ITransactionService
{
    Task<IReadOnlyCollection<TransactionResponseDto>> GetTransactionsAsync(Guid userId, int? year, int? month, CancellationToken cancellationToken = default);

    Task<TransactionResponseDto> CreateTransactionAsync(Guid userId, CreateTransactionRequestDto request, CancellationToken cancellationToken = default);

    Task<TransactionResponseDto> UpdateTransactionAsync(Guid userId, int transactionId, UpdateTransactionRequestDto request, CancellationToken cancellationToken = default);

    Task DeleteTransactionAsync(Guid userId, int transactionId, CancellationToken cancellationToken = default);
}
