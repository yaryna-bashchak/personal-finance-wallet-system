using System.Linq.Expressions;
using Moq;
using PFWS.BusinessLogicLayer.Services.Implementation;
using PFWS.DataAccessLayer.Models;
using PFWS.DataAccessLayer.Repositories;

namespace PFWS.BusinessLogicLayer.Tests;

public class GetTransactionByIdTests
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
    private Category expenseCategory = new() { Id = 1, Name = "Food", Type = "expense" };
    private Transaction defaultTransaction = new() { Id = 1, FromAccountId = 1, ExpenseCategoryId = 1 };

    [SetUp]
    public void Setup()
    {
        _mockTransactionRepository = new Mock<IRepositoryBase<Transaction>>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockAccountRepository = new Mock<IRepositoryBase<Account>>();
        _mockCategoryRepository = new Mock<IRepositoryBase<Category>>();

        _mockUserRepository.Setup(repo => repo.FindByNameAsync(username)).ReturnsAsync(user);
        _mockTransactionRepository.Setup(repo => repo.GetItemAsync(defaultTransaction.Id)).ReturnsAsync(defaultTransaction);
        _mockAccountRepository.Setup(repo => repo.GetItemAsync(account.Id)).ReturnsAsync(account);
        _mockAccountRepository.Setup(repo => repo.GetItemAsync(accountOfDifferentUser.Id)).ReturnsAsync(accountOfDifferentUser);
        _mockCategoryRepository.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync(new List<Category>() { expenseCategory });

        _transactionService = new TransactionService(_mockTransactionRepository.Object, _mockUserRepository.Object, _mockAccountRepository.Object, _mockCategoryRepository.Object);
    }

    [Test]
    public async Task GetTransactionById_WhenCalled_ReturnsTransaction()
    {
        var result = await _transactionService.GetTransactionById(defaultTransaction.Id, username);

        Assert.IsNotNull(result);
        Assert.That(result.Id, Is.EqualTo(defaultTransaction.Id));
        Assert.That(result.ExpenseCategory.Id, Is.EqualTo(expenseCategory.Id));
        Assert.That(result.ExpenseCategory.Name, Is.EqualTo(expenseCategory.Name));
    }

    [Test]
    public void GetTransactionById_WhenUserNotFound_ThrowsException()
    {
        string username = "nonExistentUser";

        Assert.ThrowsAsync<Exception>(async () => await _transactionService.GetTransactionById(defaultTransaction.Id, username));
    }

    [Test]
    public void GetTransactionById_WhenTransactionNotFound_ThrowsException()
    {
        int nonExistentTransactionId = 3;

        Assert.ThrowsAsync<Exception>(async () => await _transactionService.GetTransactionById(nonExistentTransactionId, username));
    }

    [Test]
    public void GetTransactionById_WhenUserNotAuthorized_ThrowsException()
    {
        var differentUser = new User { Id = 2, UserName = username };

        _mockUserRepository.Setup(repo => repo.FindByNameAsync(username)).ReturnsAsync(differentUser);

        Assert.ThrowsAsync<Exception>(async () => await _transactionService.GetTransactionById(defaultTransaction.Id, username));
    }

    [Test]
    public void GetTransactionById_WhenRepositoryThrowsException_ThrowsException()
    {
        _mockTransactionRepository.Setup(repo => repo.GetItemAsync(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

        Assert.ThrowsAsync<Exception>(async () => await _transactionService.GetTransactionById(1, username));
    }
}