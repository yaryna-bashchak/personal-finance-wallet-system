using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFWS.API.Controllers.Controllers;
using PFWS.API.DTOs.Account;
using PFWS.BusinessLogicLayer.Services;
using PFWS.DataAccessLayer.Data;
using PFWS.DataAccessLayer.Models;

namespace PFWS.API.Controllers;

public class AccountsController : BaseApiController
{
    private readonly IAccountService _accountService;
    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<ActionResult<List<GetAccountDto>>> GetAllAccounts()
    {
        var accounts = await _accountService.GetAllAccounts();
        var accountDtos = accounts.Select(a => new GetAccountDto
        {
            Id = a.Id,
            Name = a.Name,
            Balance = a.Balance
        });

        return Ok(accountDtos);
    }
}
