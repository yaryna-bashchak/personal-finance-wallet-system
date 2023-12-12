using System.Linq.Expressions;
using Moq;
using PFWS.BusinessLogicLayer.DTOs.Transactions;
using PFWS.BusinessLogicLayer.Services.Implementation;
using PFWS.DataAccessLayer.Models;
using PFWS.DataAccessLayer.Repositories;

namespace PFWS.BusinessLogicLayer.Tests;

public class AddTransactionTests
{
    private Mock<IRepositoryBase<Transaction>> _mockTransactionRepository;
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IRepositoryBase<Account>> _mockAccountRepository;
    private Mock<IRepositoryBase<Category>> _mockCategoryRepository;
    private TransactionService _transactionService;
    private string username = "testUser";
    private Account account = new() { Id = 1, UserId = 1 };
    private Category category = new() { Id = 1, Name = "Food", Type = "expense" };


    [SetUp]
    public void Setup()
    {
        _mockTransactionRepository = new Mock<IRepositoryBase<Transaction>>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockAccountRepository = new Mock<IRepositoryBase<Account>>();
        _mockCategoryRepository = new Mock<IRepositoryBase<Category>>();

        
        var user = new User { Id = 1, UserName = username };
        _mockUserRepository.Setup(repo => repo.FindByNameAsync(username)).ReturnsAsync(user);
        _mockAccountRepository.Setup(repo => repo.GetItemAsync(It.IsAny<int>())).ReturnsAsync(account);
        _mockTransactionRepository.Setup(repo => repo.BeginTransactionAsync()).Verifiable();
        _mockTransactionRepository.Setup(repo => repo.AddItemAsync(It.IsAny<Transaction>())).Verifiable();
        _mockTransactionRepository.Setup(repo => repo.CommitAsync()).Verifiable();
        _mockCategoryRepository.Setup(repo => repo.GetItemAsync(It.IsAny<int>())).ReturnsAsync(category);

        _transactionService = new TransactionService(_mockTransactionRepository.Object, _mockUserRepository.Object, _mockAccountRepository.Object, _mockCategoryRepository.Object);
    }

    [Test]
    public async Task AddTransaction_WhenValidData_ProceedsSuccessfully()
    {
        var newTransaction = new AddTransactionDto
        {
            FromAccountId = 1,
            ExpenseCategoryId = 1,
            Amount = 100
        };
        // string username = "testUser";
        // var user = new User { Id = 1, UserName = username };
        var account = new Account { Id = 1, UserId = 1 };
        var category = new Category { Id = 1, Name = "Food", Type = "expense" };

        // _mockUserRepository.Setup(repo => repo.FindByNameAsync(username)).ReturnsAsync(user);
        _mockAccountRepository.Setup(repo => repo.GetItemAsync(It.IsAny<int>())).ReturnsAsync(account);
        _mockTransactionRepository.Setup(repo => repo.BeginTransactionAsync()).Verifiable();
        _mockTransactionRepository.Setup(repo => repo.AddItemAsync(It.IsAny<Transaction>())).Verifiable();
        _mockTransactionRepository.Setup(repo => repo.CommitAsync()).Verifiable();
        _mockCategoryRepository.Setup(repo => repo.GetItemAsync(It.IsAny<int>())).ReturnsAsync(category);

        await _transactionService.AddTransaction(newTransaction, username);

        _mockTransactionRepository.Verify(repo => repo.BeginTransactionAsync(), Times.Once);
        _mockTransactionRepository.Verify(repo => repo.AddItemAsync(It.IsAny<Transaction>()), Times.Once);
        _mockTransactionRepository.Verify(repo => repo.CommitAsync(), Times.Once);
    }

    [Test]
    public void AddTransaction_WhenUserUnauthorized_ThrowsException()
    {
        var newTransaction = new AddTransactionDto
        {
            FromAccountId = 1,
            ExpenseCategoryId = 1,
            Amount = 100
        };
        string username = "testUser";
        string expectedExceptionMessage = "User not found";

        _mockUserRepository.Setup(repo => repo.FindByNameAsync(username)).ReturnsAsync((User)null);

        var ex = Assert.ThrowsAsync<Exception>(async () => await _transactionService.AddTransaction(newTransaction, username));

        Assert.That(ex.Message, Is.EqualTo(expectedExceptionMessage));
    }

    [Test]
    public void AddTransaction_AccountOfDifferentUserInTransaction_ThrowsException()
    {
        var newTransaction = new AddTransactionDto
        {
            FromAccountId = 1,
            ExpenseCategoryId = 1,
            Amount = 100
        };
        string username = "testUser";
        var user = new User { Id = 1, UserName = username };
        var account = new Account { Id = 1, UserId = 2 }; // different user
        string expectedExceptionMessage = "Unauthorized access to the account";

        _mockUserRepository.Setup(repo => repo.FindByNameAsync(username)).ReturnsAsync(user);
        _mockAccountRepository.Setup(repo => repo.GetItemAsync(It.IsAny<int>())).ReturnsAsync(account);

        var ex = Assert.ThrowsAsync<Exception>(async () => await _transactionService.AddTransaction(newTransaction, username));

        Assert.That(ex.Message, Is.EqualTo(expectedExceptionMessage));
    }

    [Test]
    public void AddTransaction_NoAccountIdSpecified_ThrowsException()
    {
        var newTransaction = new AddTransactionDto
        {
            ExpenseCategoryId = 1,
            Amount = 100
        };
        string username = "testUser";
        var user = new User { Id = 1, UserName = username };
        string expectedExceptionMessage = "There must be at least one account ID in transaction (FromAccountId or ToAccountId)";

        _mockUserRepository.Setup(repo => repo.FindByNameAsync(username)).ReturnsAsync(user);

        var ex = Assert.ThrowsAsync<Exception>(async () => await _transactionService.AddTransaction(newTransaction, username));

        Assert.That(ex.Message, Is.EqualTo(expectedExceptionMessage));
    }

    [Test]
    public void AddTransaction_AmountIsLessThanZero_ThrowsException()
    {
        var newTransaction = new AddTransactionDto
        {
            FromAccountId = 1,
            ExpenseCategoryId = 1,
            Amount = -100
        };
        string username = "testUser";
        var user = new User { Id = 1, UserName = username };
        var account = new Account { Id = 1, UserId = user.Id };
        string expectedExceptionMessage = "Amount must be greater than 0";

        _mockUserRepository.Setup(repo => repo.FindByNameAsync(username)).ReturnsAsync(user);
        _mockAccountRepository.Setup(repo => repo.GetItemAsync(It.IsAny<int>())).ReturnsAsync(account);

        var ex = Assert.ThrowsAsync<Exception>(async () => await _transactionService.AddTransaction(newTransaction, username));

        Assert.That(ex.Message, Is.EqualTo(expectedExceptionMessage));
    }

    [Test]
    public void AddTransaction_CategoryIdIsNotSpecified_ThrowsException()
    {
        var newTransaction = new AddTransactionDto
        {
            ToAccountId = 2,
            FromAccountId = 1,
            ExpenseCategoryId = 1,
            Amount = 100
        };
        string username = "testUser";
        var user = new User { Id = 1, UserName = username };
        var account1 = new Account { Id = 1, UserId = user.Id };
        var account2 = new Account { Id = 2, UserId = user.Id };
        string expectedExceptionMessage = "IncomeCategoryId must be specified if there is ToAccountId";

        _mockUserRepository.Setup(repo => repo.FindByNameAsync(username)).ReturnsAsync(user);
        _mockAccountRepository.Setup(repo => repo.GetItemAsync(1)).ReturnsAsync(account1);
        _mockAccountRepository.Setup(repo => repo.GetItemAsync(2)).ReturnsAsync(account2);

        var ex = Assert.ThrowsAsync<Exception>(async () => await _transactionService.AddTransaction(newTransaction, username));

        Assert.That(ex.Message, Is.EqualTo(expectedExceptionMessage));
    }

    [Test]
    public void AddTransaction_CategoryNotFound_ThrowsException()
    {
        var newTransaction = new AddTransactionDto
        {
            FromAccountId = 1,
            ExpenseCategoryId = 1,
            Amount = 100
        };
        string username = "testUser";
        var user = new User { Id = 1, UserName = username };
        var account = new Account { Id = 1, UserId = user.Id };
        string expectedExceptionMessage = "Category not found";

        _mockUserRepository.Setup(repo => repo.FindByNameAsync(username)).ReturnsAsync(user);
        _mockAccountRepository.Setup(repo => repo.GetItemAsync(1)).ReturnsAsync(account);
        _mockCategoryRepository.Setup(repo => repo.GetItemAsync(It.IsAny<int>())).ReturnsAsync((Category)null);

        var ex = Assert.ThrowsAsync<Exception>(async () => await _transactionService.AddTransaction(newTransaction, username));

        Assert.That(ex.Message, Is.EqualTo(expectedExceptionMessage));
    }

    [Test]
    public void AddTransaction_CategoryTypeIsInvalid_ThrowsException()
    {
        var newTransaction = new AddTransactionDto
        {
            FromAccountId = 1,
            ExpenseCategoryId = 4,
            Amount = 100
        };
        string username = "testUser";
        var user = new User { Id = 1, UserName = username };
        var account = new Account { Id = 1, UserId = user.Id };
        var category = new Category { Id = 4, Name = "Salary", Type = "income" };
        string expectedExceptionMessage = "ExpenseCategoryId must be id of category with type 'expense'";

        _mockUserRepository.Setup(repo => repo.FindByNameAsync(username)).ReturnsAsync(user);
        _mockAccountRepository.Setup(repo => repo.GetItemAsync(1)).ReturnsAsync(account);
        _mockCategoryRepository.Setup(repo => repo.GetItemAsync(It.IsAny<int>())).ReturnsAsync(category);

        var ex = Assert.ThrowsAsync<Exception>(async () => await _transactionService.AddTransaction(newTransaction, username));

        Assert.That(ex.Message, Is.EqualTo(expectedExceptionMessage));
    }

    [Test]
    public void AddTransaction_FromAccountIdAndToAccountIdAreTheSame_ThrowsException()
    {
        var newTransaction = new AddTransactionDto
        {
            ToAccountId = 1,
            FromAccountId = 1,
            IncomeCategoryId = 4,
            ExpenseCategoryId = 1,
            Amount = 100
        };
        string username = "testUser";
        var user = new User { Id = 1, UserName = username };
        var account1 = new Account { Id = 1, UserId = user.Id };
        var account2 = new Account { Id = 2, UserId = user.Id };
        var category1 = new Category { Id = 1, Name = "Food", Type = "expense" };
        var category2 = new Category { Id = 4, Name = "Salary", Type = "income" };
        string expectedExceptionMessage = "FromAccountId and ToAccountId can not be the same";

        _mockUserRepository.Setup(repo => repo.FindByNameAsync(username)).ReturnsAsync(user);
        _mockAccountRepository.Setup(repo => repo.GetItemAsync(1)).ReturnsAsync(account1);
        _mockAccountRepository.Setup(repo => repo.GetItemAsync(2)).ReturnsAsync(account2);
        _mockCategoryRepository.Setup(repo => repo.GetItemAsync(1)).ReturnsAsync(category1);
        _mockCategoryRepository.Setup(repo => repo.GetItemAsync(4)).ReturnsAsync(category2);

        var ex = Assert.ThrowsAsync<Exception>(async () => await _transactionService.AddTransaction(newTransaction, username));

        Assert.That(ex.Message, Is.EqualTo(expectedExceptionMessage));
    }
}