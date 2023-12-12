using System.Linq.Expressions;
using Moq;
using PFWS.BusinessLogicLayer.Services.Implementation;
using PFWS.DataAccessLayer.Models;
using PFWS.DataAccessLayer.Repositories;

namespace PFWS.BusinessLogicLayer.Tests;

public class DeleteTransactionTests
{
    private Mock<IRepositoryBase<Transaction>> _mockTransactionRepository;
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IRepositoryBase<Account>> _mockAccountRepository;
    private Mock<IRepositoryBase<Category>> _mockCategoryRepository;
    private TransactionService _transactionService;
    private string username = "testUser";
    private User user = new User { Id = 1, UserName = "testUser" };
    private User differentUser = new User { Id = 2, UserName = "differentUser" };
    private Account account = new Account { Id = 1, UserId = 1 };
    private Transaction defaultTransaction = new Transaction { Id = 1, FromAccountId = 1, ExpenseCategoryId = 1 };

    [SetUp]
    public void Setup()
    {
        _mockTransactionRepository = new Mock<IRepositoryBase<Transaction>>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockAccountRepository = new Mock<IRepositoryBase<Account>>();
        _mockCategoryRepository = new Mock<IRepositoryBase<Category>>();

        _mockUserRepository.Setup(repo => repo.FindByNameAsync(username)).ReturnsAsync(user);
        _mockUserRepository.Setup(repo => repo.FindByNameAsync(differentUser.UserName)).ReturnsAsync(differentUser);
        _mockTransactionRepository.Setup(repo => repo.GetItemAsync(defaultTransaction.Id)).ReturnsAsync(defaultTransaction);
        _mockAccountRepository.Setup(repo => repo.GetItemAsync(account.Id)).ReturnsAsync(account);

        _transactionService = new TransactionService(_mockTransactionRepository.Object, _mockUserRepository.Object, _mockAccountRepository.Object, _mockCategoryRepository.Object);
    }

    [Test]
    public async Task DeleteTransaction_WhenCalled_ProceedsSuccessfully()
    {
        await _transactionService.DeleteTransaction(defaultTransaction.Id, username);

        _mockTransactionRepository.Verify(repo => repo.DeleteItemAsync(It.IsAny<Transaction>()), Times.Once);
        _mockTransactionRepository.Verify(repo => repo.CommitAsync(), Times.Once);
    }

    [Test]
    public void DeleteTransaction_WhenUserNotFound_ThrowsException()
    {
        string nonExistentUser = "nonExistentUser";
        
        Assert.ThrowsAsync<Exception>(async () => await _transactionService.DeleteTransaction(defaultTransaction.Id, nonExistentUser));
    }

    [Test]
    public void DeleteTransaction_WhenTransactionNotFound_ThrowsException()
    {
        int nonExistentTransactionId = 10;

        Assert.ThrowsAsync<Exception>(async () => await _transactionService.DeleteTransaction(nonExistentTransactionId, username));
    }

    [Test]
    public void DeleteTransaction_WhenUserNotAuthorized_ThrowsException()
    {
        Assert.ThrowsAsync<Exception>(async () => await _transactionService.DeleteTransaction(defaultTransaction.Id, differentUser.UserName));
    }

    [Test]
    public void DeleteTransaction_WhenRepositoryThrowsException_ThrowsException()
    {
        _mockTransactionRepository.Setup(repo => repo.DeleteItemAsync(It.IsAny<Transaction>())).ThrowsAsync(new Exception("Database error"));

        Assert.ThrowsAsync<Exception>(async () => await _transactionService.DeleteTransaction(defaultTransaction.Id, username));
        _mockTransactionRepository.Verify(repo => repo.Rollback(), Times.Once);
    }
}
