using System.Linq.Expressions;
using Moq;
using PFWS.BusinessLogicLayer.DTOs.Transactions;
using PFWS.BusinessLogicLayer.Services.Implementation;
using PFWS.DataAccessLayer.Models;
using PFWS.DataAccessLayer.Repositories;

namespace PFWS.BusinessLogicLayer.Tests;

public class GetTransactionsByAccountIdTests
{
    private Mock<IRepositoryBase<Transaction>> _mockTransactionRepository;
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IRepositoryBase<Account>> _mockAccountRepository;
    private Mock<IRepositoryBase<Category>> _mockCategoryRepository;
    private TransactionService _transactionService;
    private string username = "testUser";
    private User user = new() { Id = 1, UserName = "testUser" };
    private Account account = new() { Id = 1, UserId = 1 };
    private Account accountOfDifferentUser = new() { Id = 3, UserId = 2 };
    private List<Category> defaultCategories = new()
    {
        new() { Id = 1, Name = "Food", Type = "expense" },
        new() { Id = 4, Name = "Salary", Type = "income" },
    };
    private List<Transaction> defaultTransactions = new()
    {
        new() { Id = 1, FromAccountId = 1, ExpenseCategoryId = 1 },
        new() { Id = 2, ToAccountId = 1, IncomeCategoryId = 4 },
    };

    [SetUp]
    public void Setup()
    {
        _mockTransactionRepository = new Mock<IRepositoryBase<Transaction>>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockAccountRepository = new Mock<IRepositoryBase<Account>>();
        _mockCategoryRepository = new Mock<IRepositoryBase<Category>>();

        _mockUserRepository.Setup(repo => repo.FindByNameAsync(username)).ReturnsAsync(user);
        _mockAccountRepository.Setup(repo => repo.GetItemAsync(account.Id)).ReturnsAsync(account);
        _mockAccountRepository.Setup(repo => repo.GetItemAsync(accountOfDifferentUser.Id)).ReturnsAsync(accountOfDifferentUser);
        _mockCategoryRepository.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync(defaultCategories);
        _mockTransactionRepository.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Transaction, bool>>>())).ReturnsAsync(defaultTransactions);

        _transactionService = new TransactionService(_mockTransactionRepository.Object, _mockUserRepository.Object, _mockAccountRepository.Object, _mockCategoryRepository.Object);
    }

    [Test]
    public async Task GetTransactionsByAccountId_WhenUserAndAccountExist_ReturnsTransactions()
    {
        var result = await _transactionService.GetTransactionsByAccountId(account.Id, username);

        Assert.IsNotNull(result);
        Assert.IsNotEmpty(result);
        Assert.That(result.Count, Is.EqualTo(defaultTransactions.Count));
        Assert.That(result[0].Id, Is.EqualTo(1));
        Assert.That(result[1].Id, Is.EqualTo(2));
        Assert.That(result[0].ExpenseCategory.Id, Is.EqualTo(defaultTransactions.First(t => t.Id == 1).ExpenseCategoryId));
        Assert.That(result[1].IncomeCategory.Id, Is.EqualTo(defaultTransactions.First(t => t.Id == 2).IncomeCategoryId));
    }

    [Test]
    public void GetTransactionsByAccountId_WhenUserNotFound_ThrowsException()
    {
        string username = "nonExistentUser";

        Assert.ThrowsAsync<Exception>(async () => await _transactionService.GetTransactionsByAccountId(account.Id, username));
    }

    [Test]
    public void GetTransactionsByAccountId_WhenAccountNotFound_ThrowsException()
    {
        int accountId = 10;

        Assert.ThrowsAsync<Exception>(async () => await _transactionService.GetTransactionsByAccountId(accountId, username));
    }

    [Test]
    public void GetTransactionsByAccountId_WhenUserNotAuthorized_ThrowsException()
    {
        int accountId = 3; // account of different user

        Assert.ThrowsAsync<Exception>(async () => await _transactionService.GetTransactionsByAccountId(accountId, username));
    }

    [Test]
    public async Task GetTransactionsByAccountId_WhenNoTransactionsFound_ReturnsEmptyList()
    {
        _mockTransactionRepository.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Transaction, bool>>>())).ReturnsAsync(new List<Transaction>());

        var result = await _transactionService.GetTransactionsByAccountId(account.Id, username);

        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }

    [Test]
    public void GetTransactionsByAccountId_WhenRepositoryThrowsException_ThrowsException()
    {
        _mockTransactionRepository.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Transaction, bool>>>())).ThrowsAsync(new Exception("Database error"));

        Assert.ThrowsAsync<Exception>(async () => await _transactionService.GetTransactionsByAccountId(account.Id, username));
    }
}