using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PFWS.API.Controllers;
using PFWS.BusinessLogicLayer.DTOs.Account;
using PFWS.BusinessLogicLayer.Services;

namespace PFWS.API.Tests;

public class AccountsControllerTests
{
    private Mock<IAccountService> mockService;
    private AccountsController controller;
    private string username = "testUser";

    [SetUp]
    public void Setup()
    {
        mockService = new();
        controller = new AccountsController(mockService.Object);
        SetupControllerHttpContext(controller, username);
    }

    [Test]
    public async Task GetAccountById_WithValidAccount_ReturnsOk()
    {
        var expectedAccount = new GetAccountDto { Id = 1, UserId = 1, Balance = 1000 };
        mockService.Setup(s => s.GetAccountById(It.IsAny<int>(), username))
                   .ReturnsAsync(expectedAccount);

        var result = await controller.GetAccountById(1);

        Assert.IsInstanceOf<OkObjectResult>(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(expectedAccount));
    }


    [Test]
    public async Task GetUserAccounts_WithValidAccounts_ReturnsOk()
    {
        var expectedAccounts = new List<GetAccountDto>
        {
            new GetAccountDto { Id = 1, UserId = 1, Balance = 1000 },
            new GetAccountDto { Id = 2, UserId = 1, Balance = 2000 }
        };

        mockService.Setup(s => s.GetUserAccounts(username))
                   .ReturnsAsync(expectedAccounts);

        var result = await controller.GetUserAccounts();

        Assert.IsInstanceOf<OkObjectResult>(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(expectedAccounts));
    }

    [Test]
    public async Task AddAccount_WithValidAccount_ReturnsOk()
    {
        var newAccount = new AddAccountDto { Name = "Account name", Balance = 1000 };
        mockService.Setup(s => s.AddAccount(It.IsAny<AddAccountDto>(), username))
                   .Returns(Task.CompletedTask);

        var result = await controller.AddAccount(newAccount);

        Assert.IsInstanceOf<OkResult>(result);
    }

    [Test]
    public async Task UpdateAccount_WithValidAccount_ReturnsNoContent()
    {
        var updatedAccount = new UpdateAccountDto { Name = "Updated account name" };
        mockService.Setup(s => s.UpdateAccount(It.IsAny<int>(), It.IsAny<UpdateAccountDto>(), username))
                   .Returns(Task.CompletedTask);

        var result = await controller.UpdateAccount(1, updatedAccount);

        Assert.IsInstanceOf<NoContentResult>(result);
    }

    [Test]
    public async Task DeleteAccount_WithValidId_ReturnsNoContent()
    {
        mockService.Setup(s => s.DeleteAccount(It.IsAny<int>(), username))
                   .Returns(Task.CompletedTask);

        var result = await controller.DeleteAccount(1);

        Assert.IsInstanceOf<NoContentResult>(result);
    }

    private static void SetupControllerHttpContext(ControllerBase controller, string username)
    {
        var context = new DefaultHttpContext();
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        context.User = principal;

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = context
        };
    }
}