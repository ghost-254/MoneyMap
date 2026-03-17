using MoneyMap.Api.DTOs.Reports;

namespace MoneyMap.Api.Services.Interfaces;

public interface IReportService
{
    Task<MonthlySummaryDto> GetMonthlySummaryAsync(Guid userId, int? year, int? month, CancellationToken cancellationToken = default);
}
