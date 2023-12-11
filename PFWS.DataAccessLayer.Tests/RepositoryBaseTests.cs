namespace PFWS.DataAccessLayer.Tests;

public class RepositoryBaseTests
{
    private Mock<DbSet<Account>> mockSet;
    private Mock<WalletContext> mockContext;
    private RepositoryBase<Account> repository;

    [SetUp]
    public void Setup()
    {
        var data = new List<Account>
        {
            new Account { Id = 1, Name = "Account 1", Balance = 1000, UserId = 1 },
            new Account { Id = 2, Name = "Account 2", Balance = 1500, UserId = 1 },
            new Account { Id = 3, Name = "Account 3", Balance = 500, UserId = 2 }
        };

        var queryableData = data.AsQueryable();

        mockSet = new Mock<DbSet<Account>>();
        mockSet.As<IQueryable<Account>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Account>(queryableData.Provider));
        mockSet.As<IQueryable<Account>>().Setup(m => m.Expression).Returns(queryableData.Expression);
        mockSet.As<IQueryable<Account>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
        mockSet.As<IQueryable<Account>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator);

        mockSet.As<IAsyncEnumerable<Account>>().Setup(d => d.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<Account>(data.GetEnumerator()));

        mockContext = new Mock<WalletContext>();
        mockContext.Setup(c => c.Set<Account>()).Returns(mockSet.Object);
        // mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        
        repository = new RepositoryBase<Account>(mockContext.Object);
    }

    [Test]
    public async Task GetAllItemsAsync_ReturnsAllItems()
    {
        int expectedCount = 3;
        var expectedAccounts = new List<Account>
        {
            new Account { Id = 1, Name = "Account 1", Balance = 1000, UserId = 1 },
            new Account { Id = 2, Name = "Account 2", Balance = 1500, UserId = 1 },
            new Account { Id = 3, Name = "Account 3", Balance = 500, UserId = 2 }
        };

        var result = await repository.GetAllItemsAsync();

        Assert.IsNotNull(result);
        Assert.That(result, Has.Count.EqualTo(expectedCount));
        CollectionAssert.AreEqual(expectedAccounts, result, new AccountComparer());
    }

    [Test]
    public async Task GetItemAsync_ReturnsCorrectItem()
    {
        var expectedAccount = new Account { Id = 1, Name = "Account 1", Balance = 1000, UserId = 1 };
        mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
               .ReturnsAsync(expectedAccount);

        var result = await repository.GetItemAsync(1);

        Assert.IsNotNull(result);
        var comparer = new AccountComparer();
        Assert.IsTrue(comparer.Equals(expectedAccount, result));
    }
}