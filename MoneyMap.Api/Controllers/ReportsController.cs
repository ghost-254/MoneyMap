using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMap.Api.DTOs.Reports;
using MoneyMap.Api.Extensions;
using MoneyMap.Api.Services.Interfaces;

namespace MoneyMap.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/reports")]
public sealed class ReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet("monthly-summary")]
    [ProducesResponseType(typeof(MonthlySummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MonthlySummaryDto>> GetMonthlySummary(
        [FromQuery] int? year,
        [FromQuery] int? month,
        CancellationToken cancellationToken)
    {
        var summary = await reportService.GetMonthlySummaryAsync(User.GetUserId(), year, month, cancellationToken);
        return Ok(summary);
    }
}
