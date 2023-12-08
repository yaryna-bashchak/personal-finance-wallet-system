using Microsoft.AspNetCore.Mvc;
using PFWS.API.Controllers.Controllers;
using PFWS.BusinessLogicLayer.DTOs.Transactions;
using PFWS.BusinessLogicLayer.Services;

namespace PFWS.API.Controllers;

public class TransactionController : BaseApiController
{
    private readonly ITransactionService _transactionService;
    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetTransactionDto>> GetTransactionById(int id)
    {
        try
        {
            var username = User.Identity.Name;
            var transaction = await _transactionService.GetTransactionById(id, username);
            return Ok(transaction);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("accountId/{accountId}")]
    public async Task<ActionResult<List<GetTransactionDto>>> GetTransactionsByAccountId(int accountId)
    {
        try
        {
            var username = User.Identity.Name;
            var transactions = await _transactionService.GetTransactionsByAccountId(accountId, username);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult> AddTransaction(AddTransactionDto newTransaction)
    {
        try
        {
            var username = User.Identity.Name;
            await _transactionService.AddTransaction(newTransaction, username);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransaction(int id, UpdateTransactionDto updatedTransaction)
    {
        try
        {
            var username = User.Identity.Name;
            await _transactionService.UpdateTransaction(id, updatedTransaction, username);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        try
        {
            var username = User.Identity.Name;
            await _transactionService.DeleteTransaction(id, username);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
