using Microsoft.EntityFrameworkCore;
using MoneyMap.Api.Data;
using MoneyMap.Api.DTOs.Reports;
using MoneyMap.Api.Exceptions;
using MoneyMap.Api.Models;
using MoneyMap.Api.Services.Interfaces;

namespace MoneyMap.Api.Services;

public sealed class ReportService(AppDbContext dbContext) : IReportService
{
    public async Task<MonthlySummaryDto> GetMonthlySummaryAsync(Guid userId, int? year, int? month, CancellationToken cancellationToken = default)
    {
        var (periodStartUtc, periodEndUtc) = ResolvePeriod(year, month);

        var transactions = await dbContext.Transactions
            .AsNoTracking()
            .Include(transaction => transaction.Category)
            .Where(transaction =>
                transaction.UserId == userId &&
                transaction.OccurredOn >= periodStartUtc &&
                transaction.OccurredOn < periodEndUtc)
            .ToListAsync(cancellationToken);

        var totalIncome = transactions
            .Where(transaction => transaction.Category.Type == CategoryType.Income)
            .Sum(transaction => transaction.Amount);

        var totalExpenses = transactions
            .Where(transaction => transaction.Category.Type == CategoryType.Expense)
            .Sum(transaction => transaction.Amount);

        var breakdown = transactions
            .GroupBy(transaction => new
            {
                transaction.CategoryId,
                transaction.Category.Name,
                transaction.Category.Type
            })
            .Select(group => new CategoryBreakdownDto
            {
                CategoryId = group.Key.CategoryId,
                CategoryName = group.Key.Name,
                Type = group.Key.Type,
                TotalAmount = group.Sum(transaction => transaction.Amount)
            })
            .OrderBy(item => item.Type)
            .ThenByDescending(item => item.TotalAmount)
            .ThenBy(item => item.CategoryName)
            .ToList();

        return new MonthlySummaryDto
        {
            Year = periodStartUtc.Year,
            Month = periodStartUtc.Month,
            PeriodStartUtc = periodStartUtc,
            PeriodEndUtc = periodEndUtc,
            TransactionCount = transactions.Count,
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            Net = totalIncome - totalExpenses,
            Breakdown = breakdown
        };
    }

    private static (DateTime PeriodStartUtc, DateTime PeriodEndUtc) ResolvePeriod(int? year, int? month)
    {
        if (year is null && month is null)
        {
            var now = DateTime.UtcNow;
            var start = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            return (start, start.AddMonths(1));
        }

        if (year is null || month is null)
        {
            throw new BadRequestException("Both year and month must be supplied together.");
        }

        if (year < 2000 || year > 2100 || month < 1 || month > 12)
        {
            throw new BadRequestException("Invalid year or month value.");
        }

        var periodStart = new DateTime(year.Value, month.Value, 1, 0, 0, 0, DateTimeKind.Utc);
        return (periodStart, periodStart.AddMonths(1));
    }
}
