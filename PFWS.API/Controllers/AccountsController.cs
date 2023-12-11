using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PFWS.API.Controllers.Controllers;
using PFWS.BusinessLogicLayer.DTOs.Account;
using PFWS.BusinessLogicLayer.Services;

namespace PFWS.API.Controllers;

[Authorize]
public class AccountsController : BaseApiController
{
    private readonly IAccountService _accountService;
    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetAccountDto>> GetAccountById(int id)
    {
        try
        {
            var username = User.Identity.Name;
            var account = await _accountService.GetAccountById(id, username);
            return Ok(account);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<GetAccountDto>>> GetUserAccounts()
    {
        try
        {
            var username = User.Identity.Name;
            var accounts = await _accountService.GetUserAccounts(username);
            return Ok(accounts);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult> AddAccount(AddAccountDto newAccount)
    {
        try
        {
            var username = User.Identity.Name;
            await _accountService.AddAccount(newAccount, username);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAccount(int id, UpdateAccountDto updatedAccount)
    {
        try
        {
            var username = User.Identity.Name;
            await _accountService.UpdateAccount(id, updatedAccount, username);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        try
        {
            var username = User.Identity.Name;
            await _accountService.DeleteAccount(id, username);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
