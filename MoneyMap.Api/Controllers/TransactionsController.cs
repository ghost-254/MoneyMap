using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMap.Api.DTOs.Transactions;
using MoneyMap.Api.Extensions;
using MoneyMap.Api.Services.Interfaces;

namespace MoneyMap.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/transactions")]
public sealed class TransactionsController(ITransactionService transactionService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<TransactionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyCollection<TransactionResponseDto>>> GetTransactions(
        [FromQuery] int? year,
        [FromQuery] int? month,
        CancellationToken cancellationToken)
    {
        var transactions = await transactionService.GetTransactionsAsync(User.GetUserId(), year, month, cancellationToken);
        return Ok(transactions);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TransactionResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TransactionResponseDto>> CreateTransaction(
        CreateTransactionRequestDto request,
        CancellationToken cancellationToken)
    {
        var transaction = await transactionService.CreateTransactionAsync(User.GetUserId(), request, cancellationToken);
        return Created($"/api/transactions/{transaction.Id}", transaction);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TransactionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionResponseDto>> UpdateTransaction(
        int id,
        UpdateTransactionRequestDto request,
        CancellationToken cancellationToken)
    {
        var transaction = await transactionService.UpdateTransactionAsync(User.GetUserId(), id, request, cancellationToken);
        return Ok(transaction);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTransaction(int id, CancellationToken cancellationToken)
    {
        await transactionService.DeleteTransactionAsync(User.GetUserId(), id, cancellationToken);
        return NoContent();
    }
}
