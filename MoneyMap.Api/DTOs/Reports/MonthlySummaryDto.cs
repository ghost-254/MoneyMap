namespace MoneyMap.Api.DTOs.Reports;

public sealed class MonthlySummaryDto
{
    public int Year { get; init; }

    public int Month { get; init; }

    public DateTime PeriodStartUtc { get; init; }

    public DateTime PeriodEndUtc { get; init; }

    public int TransactionCount { get; init; }

    public decimal TotalIncome { get; init; }

    public decimal TotalExpenses { get; init; }

    public decimal Net { get; init; }

    public IReadOnlyCollection<CategoryBreakdownDto> Breakdown { get; init; } = [];
}
