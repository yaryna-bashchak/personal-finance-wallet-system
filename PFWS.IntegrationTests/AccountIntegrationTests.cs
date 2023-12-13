using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PFWS.API.Controllers;
using PFWS.BusinessLogicLayer.DTOs.Account;
using PFWS.BusinessLogicLayer.Services.Implementation;
using PFWS.DataAccessLayer.Models;
using PFWS.DataAccessLayer.Repositories;

namespace PFWS.IntegrationTests;

[TestFixture]
public class AccountIntegrationTests
{
    private Mock<AccountService> mockService;
    private Mock<IRepositoryBase<Account>> mockAccountRepository;
    private Mock<IUserRepository> mockUserRepository;
    private AccountsController controller;
    private readonly string username = "testUser";
    private readonly User expectedUser = new() { Id = 1, UserName = "testUser" };
    private readonly Account dbAccount = new() { Id = 1, UserId = 1, Balance = 1000 };

    [SetUp]
    public void Setup()
    {
        mockAccountRepository = new();
        mockUserRepository = new();

        mockUserRepository.Setup(s => s.FindByNameAsync(username))
                            .ReturnsAsync(expectedUser);
        mockAccountRepository.Setup(s => s.GetItemAsync(It.IsAny<int>()))
                   .ReturnsAsync(dbAccount);

        mockService = new(mockAccountRepository.Object, mockUserRepository.Object);
        controller = new(mockService.Object);
        SetupControllerHttpContext(controller, username);
    }

    [Test]
    public async Task GetAccountById_ReturnsCorrectData()
    {
        var dbAccount = new Account { Id = 1, UserId = 1, Balance = 1000 };

        var result = await controller.GetAccountById(1);

        var expectedAccount = new GetAccountDto { Id = 1, UserId = 1, Balance = 1000 };
        Assert.IsInstanceOf<OkObjectResult>(result.Result);
        var okResult = result.Result as OkObjectResult;
        var responseAccount = okResult?.Value as GetAccountDto;
        Assert.That(responseAccount?.Id, Is.EqualTo(expectedAccount.Id));
        Assert.That(responseAccount?.UserId, Is.EqualTo(expectedAccount.UserId));
        Assert.That(responseAccount?.Balance, Is.EqualTo(expectedAccount.Balance));
    }

    [Test]
    public async Task GetUserAccounts_ReturnsCorrectData()
    {
        var dbAccounts = new List<Account>
        {
            new Account { Id = 1, UserId = 1, Balance = 1000 },
            new Account { Id = 2, UserId = 1, Balance = 500 },
            new Account { Id = 3, UserId = 1, Balance = 1500 },
        };
        mockAccountRepository.Setup(s => s.FindByConditionAsync(It.IsAny<Expression<Func<Account, bool>>>()))
                            .ReturnsAsync(dbAccounts);

        var result = await controller.GetUserAccounts();

        var expectedAccounts = new List<GetAccountDto>
        {
            new GetAccountDto { Id = 1, UserId = 1, Balance = 1000 },
            new GetAccountDto { Id = 2, UserId = 1, Balance = 500 },
            new GetAccountDto { Id = 3, UserId = 1, Balance = 1500 },
        };

        Assert.IsInstanceOf<OkObjectResult>(result.Result);
        var okResult = result.Result as OkObjectResult;
        var responseAccount = okResult?.Value as List<GetAccountDto>;
        Assert.That(responseAccount?.Count, Is.EqualTo(expectedAccounts.Count));
        Assert.That(responseAccount?[0].UserId, Is.EqualTo(expectedAccounts[0].UserId));
        Assert.That(responseAccount?[1].UserId, Is.EqualTo(expectedAccounts[1].UserId));
        Assert.That(responseAccount?[2].UserId, Is.EqualTo(expectedAccounts[2].UserId));
    }

    [Test]
    public async Task AddAccount_ReturnsCorrectData()
    {
        var addAccountDto = new AddAccountDto { Balance = 1000, Name = "New account" };
        mockAccountRepository.Setup(s => s.AddItemAsync(It.IsAny<Account>()))
                     .Returns(Task.CompletedTask);

        var result = await controller.AddAccount(addAccountDto);

        Assert.IsInstanceOf<OkResult>(result);
    }

    [Test]
    public async Task UpdateAccount_ReturnsCorrectData()
    {
        var accountId = 1;
        var updatedAccountDto = new UpdateAccountDto { Name = "Updated account" };
        mockAccountRepository.Setup(s => s.UpdateItemAsync(It.IsAny<int>(), It.IsAny<Account>()))
                     .Returns(Task.CompletedTask);

        var result = await controller.UpdateAccount(accountId, updatedAccountDto);

        Assert.IsInstanceOf<NoContentResult>(result);
    }

    [Test]
    public async Task DeleteAccount_ReturnsCorrectData()
    {
        var accountId = 1;
        mockAccountRepository.Setup(s => s.DeleteItemAsync(It.IsAny<Account>()))
                     .Returns(Task.CompletedTask);

        var result = await controller.DeleteAccount(accountId);

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
