using Microsoft.EntityFrameworkCore;
using MoneyMap.Api.Data;
using MoneyMap.Api.DTOs.Transactions;
using MoneyMap.Api.Exceptions;
using MoneyMap.Api.Models;
using MoneyMap.Api.Services.Interfaces;
using TransactionEntity = MoneyMap.Api.Models.Transaction;

namespace MoneyMap.Api.Services;

public sealed class TransactionService : ITransactionService
{
    private readonly AppDbContext _dbContext;

    public TransactionService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<TransactionResponseDto>> GetTransactionsAsync(Guid userId, int? year, int? month, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Transactions
            .AsNoTracking()
            .Include(transaction => transaction.Category)
            .Where(transaction => transaction.UserId == userId);

        if (year is not null || month is not null)
        {
            var period = ResolvePeriod(year, month);
            query = query.Where(transaction => transaction.OccurredOn >= period.PeriodStartUtc && transaction.OccurredOn < period.PeriodEndUtc);
        }

        return await query
            .OrderByDescending(transaction => transaction.OccurredOn)
            .ThenByDescending(transaction => transaction.Id)
            .Select(transaction => new TransactionResponseDto
            {
                Id = transaction.Id,
                CategoryId = transaction.CategoryId,
                CategoryName = transaction.Category.Name,
                CategoryType = transaction.Category.Type,
                Amount = transaction.Amount,
                Description = transaction.Description,
                OccurredOn = transaction.OccurredOn,
                CreatedAtUtc = transaction.CreatedAtUtc,
                UpdatedAtUtc = transaction.UpdatedAtUtc
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<TransactionResponseDto> CreateTransactionAsync(Guid userId, CreateTransactionRequestDto request, CancellationToken cancellationToken = default)
    {
        var category = await GetOwnedCategoryAsync(userId, request.CategoryId, cancellationToken);

        var transaction = new TransactionEntity
        {
            UserId = userId,
            CategoryId = category.Id,
            Amount = request.Amount,
            Description = NormalizeDescription(request.Description),
            OccurredOn = request.OccurredOn.ToUniversalTime(),
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync(cancellationToken);

        transaction.Category = category;
        return MapToDto(transaction);
    }

    public async Task<TransactionResponseDto> UpdateTransactionAsync(Guid userId, int transactionId, UpdateTransactionRequestDto request, CancellationToken cancellationToken = default)
    {
        var transaction = await _dbContext.Transactions
            .Include(existing => existing.Category)
            .SingleOrDefaultAsync(existing => existing.Id == transactionId && existing.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Transaction not found.");

        var category = await GetOwnedCategoryAsync(userId, request.CategoryId, cancellationToken);

        transaction.CategoryId = category.Id;
        transaction.Category = category;
        transaction.Amount = request.Amount;
        transaction.Description = NormalizeDescription(request.Description);
        transaction.OccurredOn = request.OccurredOn.ToUniversalTime();
        transaction.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return MapToDto(transaction);
    }

    public async Task DeleteTransactionAsync(Guid userId, int transactionId, CancellationToken cancellationToken = default)
    {
        var transaction = await _dbContext.Transactions
            .SingleOrDefaultAsync(existing => existing.Id == transactionId && existing.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Transaction not found.");

        _dbContext.Transactions.Remove(transaction);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Category> GetOwnedCategoryAsync(Guid userId, int categoryId, CancellationToken cancellationToken)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .SingleOrDefaultAsync(category => category.Id == categoryId && category.UserId == userId, cancellationToken)
            ?? throw new BadRequestException("The selected category does not exist.");
    }

    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    private static TransactionResponseDto MapToDto(TransactionEntity transaction) => new TransactionResponseDto
    {
        Id = transaction.Id,
        CategoryId = transaction.CategoryId,
        CategoryName = transaction.Category.Name,
        CategoryType = transaction.Category.Type,
        Amount = transaction.Amount,
        Description = transaction.Description,
        OccurredOn = transaction.OccurredOn,
        CreatedAtUtc = transaction.CreatedAtUtc,
        UpdatedAtUtc = transaction.UpdatedAtUtc
    };

    private static DateRange ResolvePeriod(int? year, int? month)
    {
        if (year is null || month is null)
        {
            throw new BadRequestException("Both year and month must be supplied together.");
        }

        if (year < 2000 || year > 2100 || month < 1 || month > 12)
        {
            throw new BadRequestException("Invalid year or month value.");
        }

        var periodStart = new DateTime(year.Value, month.Value, 1, 0, 0, 0, DateTimeKind.Utc);
        return new DateRange(periodStart, periodStart.AddMonths(1));
    }

    private sealed class DateRange
    {
        public DateRange(DateTime periodStartUtc, DateTime periodEndUtc)
        {
            PeriodStartUtc = periodStartUtc;
            PeriodEndUtc = periodEndUtc;
        }

        public DateTime PeriodStartUtc { get; }

        public DateTime PeriodEndUtc { get; }
    }
}
