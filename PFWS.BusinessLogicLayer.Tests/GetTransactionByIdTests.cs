using System.Linq.Expressions;
using Moq;
using PFWS.BusinessLogicLayer.DTOs.Transactions;
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

    [SetUp]
    public void Setup()
    {
        _mockTransactionRepository = new Mock<IRepositoryBase<Transaction>>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockAccountRepository = new Mock<IRepositoryBase<Account>>();
        _mockCategoryRepository = new Mock<IRepositoryBase<Category>>();

        _transactionService = new TransactionService(_mockTransactionRepository.Object, _mockUserRepository.Object, _mockAccountRepository.Object, _mockCategoryRepository.Object);
    }

    [Test]
    public async Task GetTransactionById_WhenCalled_ReturnsTransaction()
    {
        int transactionId = 1;
        int accountId = 1;
        string username = "testUser";
        var user = new User { Id = 1, UserName = username };
        var account = new Account { Id = accountId, UserId = user.Id };
        var transaction = new Transaction { Id = transactionId, FromAccountId = 1 };

        _mockUserRepository.Setup(repo => repo.FindByNameAsync(username)).ReturnsAsync(user);
        _mockTransactionRepository.Setup(repo => repo.GetItemAsync(transactionId)).ReturnsAsync(transaction);
        _mockAccountRepository.Setup(repo => repo.GetItemAsync(accountId)).ReturnsAsync(account);
        _mockCategoryRepository.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync(new List<Category>());

        var result = await _transactionService.GetTransactionById(transactionId, username);

        Assert.IsNotNull(result);
        Assert.That(result.Id, Is.EqualTo(transactionId));
    }

    [Test]
    public void GetTransactionById_WhenUserNotFound_ThrowsException()
    {
        int transactionId = 1;
        string username = "nonExistentUser";

        _mockUserRepository.Setup(repo => repo.FindByNameAsync(username)).ReturnsAsync((User)null);

        Assert.ThrowsAsync<Exception>(async () => await _transactionService.GetTransactionById(transactionId, username));
    }

    [Test]
    public void GetTransactionById_WhenTransactionNotFound_ThrowsException()
    {
        int transactionId = 1;
        string username = "testUser";
        var user = new User { Id = 1, UserName = username };

        _mockUserRepository.Setup(repo => repo.FindByNameAsync(username)).ReturnsAsync(user);
        _mockTransactionRepository.Setup(repo => repo.GetItemAsync(transactionId)).ReturnsAsync(null as Transaction);

        Assert.ThrowsAsync<Exception>(async () => await _transactionService.GetTransactionById(transactionId, username));
    }

    [Test]
    public void GetTransactionById_WhenUserNotAuthorized_ThrowsException()
    {
        int transactionId = 1;
        string username = "testUser";
        var user = new User { Id = 2, UserName = username }; // different user Id
        var transaction = new Transaction { Id = transactionId, FromAccountId = 1 };

        _mockUserRepository.Setup(repo => repo.FindByNameAsync(username)).ReturnsAsync(user);
        _mockTransactionRepository.Setup(repo => repo.GetItemAsync(transactionId)).ReturnsAsync(transaction);

        Assert.ThrowsAsync<Exception>(async () => await _transactionService.GetTransactionById(transactionId, username));
    }

    [Test]
    public void GetTransactionById_WhenRepositoryThrowsException_ThrowsException()
    {
        int transactionId = 1;
        string username = "testUser";
        var user = new User { Id = 1, UserName = username };

        _mockUserRepository.Setup(repo => repo.FindByNameAsync(username)).ReturnsAsync(user);
        _mockTransactionRepository.Setup(repo => repo.GetItemAsync(transactionId)).ThrowsAsync(new Exception("Database error"));

        Assert.ThrowsAsync<Exception>(async () => await _transactionService.GetTransactionById(transactionId, username));
    }
}