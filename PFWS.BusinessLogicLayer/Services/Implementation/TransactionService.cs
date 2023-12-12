using PFWS.BusinessLogicLayer.DTOs.Category;
using PFWS.BusinessLogicLayer.DTOs.Transactions;
using PFWS.DataAccessLayer.Models;
using PFWS.DataAccessLayer.Repositories;

namespace PFWS.BusinessLogicLayer.Services.Implementation;

public class TransactionService : ITransactionService
{
    private readonly IRepositoryBase<Transaction> _repositoryBase;
    private readonly IUserRepository _userRepository;
    private readonly IRepositoryBase<Account> _accountRepository;
    private readonly IRepositoryBase<Category> _categoryRepository;

    public TransactionService(IRepositoryBase<Transaction> repositoryBase, IUserRepository userRepository, IRepositoryBase<Account> accountRepository, IRepositoryBase<Category> categoryRepository)
    {
        _repositoryBase = repositoryBase;
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<List<GetTransactionDto>> GetTransactionsByAccountId(int accountId, string username)
    {
        await ValidateUserAccountAccess(accountId, username);

        var transactions = await _repositoryBase.FindByConditionAsync(t => t.FromAccountId == accountId || t.ToAccountId == accountId);
        var categoryMap = await FetchCategoriesForTransactions(transactions);

        return transactions.Select(t => MapToTransactionDto(t, categoryMap)).ToList();
    }

    public async Task<GetTransactionDto> GetTransactionById(int id, string username)
    {
        var transaction = await GetTransactionForUser(id, username);
        var categoryMap = await FetchCategoriesForTransactions(new List<Transaction> { transaction });

        return MapToTransactionDto(transaction, categoryMap);
    }

    public async Task AddTransaction(AddTransactionDto newTransaction, string username)
    {
        if (newTransaction.FromAccountId.HasValue)
        {
            await ValidateUserAccountAccess(newTransaction.FromAccountId.Value, username);
        }
        if (newTransaction.ToAccountId.HasValue)
        {
            await ValidateUserAccountAccess(newTransaction.ToAccountId.Value, username);
        }

        await ValidateTransactionData(newTransaction, newTransaction.FromAccountId, newTransaction.ToAccountId, newTransaction.Amount);

        try
        {
            await _repositoryBase.BeginTransactionAsync();

            var transaction = new Transaction
            {
                FromAccountId = newTransaction.FromAccountId,
                ToAccountId = newTransaction.ToAccountId,
                ExpenseCategoryId = newTransaction.ExpenseCategoryId,
                IncomeCategoryId = newTransaction.IncomeCategoryId,
                Amount = newTransaction.Amount,
            };

            await _repositoryBase.AddItemAsync(transaction);

            if (newTransaction.FromAccountId.HasValue)
            {
                await UpdateAccountBalance(newTransaction.FromAccountId.Value, -newTransaction.Amount);
            }
            if (newTransaction.ToAccountId.HasValue)
            {
                await UpdateAccountBalance(newTransaction.ToAccountId.Value, newTransaction.Amount);
            }

            await _repositoryBase.CommitAsync();
        }
        catch (Exception)
        {
            _repositoryBase.Rollback();
            throw;
        }
    }

    public async Task UpdateTransaction(int id, UpdateTransactionDto updatedTransaction, string username)
    {
        var originalTransaction = await GetTransactionForUser(id, username);

        if (updatedTransaction.FromAccountId.HasValue && originalTransaction.FromAccountId != updatedTransaction.FromAccountId)
        {
            await ValidateUserAccountAccess(updatedTransaction.FromAccountId.Value, username);
        }
        if (updatedTransaction.ToAccountId.HasValue && originalTransaction.ToAccountId != updatedTransaction.ToAccountId)
        {
            await ValidateUserAccountAccess(updatedTransaction.ToAccountId.Value, username);
        }

        int? fromAccountId = updatedTransaction.FromAccountId.HasValue ? updatedTransaction.FromAccountId.Value : originalTransaction.FromAccountId.Value;
        int? toAccountId = updatedTransaction.ToAccountId.HasValue ? updatedTransaction.ToAccountId.Value : originalTransaction.ToAccountId.Value;
        decimal amount = updatedTransaction.Amount == 0 ? originalTransaction.Amount : updatedTransaction.Amount;

        await ValidateTransactionData(updatedTransaction, fromAccountId, toAccountId, amount);

        try
        {
            await _repositoryBase.BeginTransactionAsync();

            if (originalTransaction.FromAccountId.HasValue)
            {
                await UpdateAccountBalance(originalTransaction.FromAccountId.Value, originalTransaction.Amount);
            }
            if (originalTransaction.ToAccountId.HasValue)
            {
                await UpdateAccountBalance(originalTransaction.ToAccountId.Value, -originalTransaction.Amount);
            }

            originalTransaction.FromAccountId = fromAccountId;
            originalTransaction.ToAccountId = updatedTransaction.ToAccountId ?? originalTransaction.ToAccountId;
            originalTransaction.ExpenseCategoryId = updatedTransaction.ExpenseCategoryId ?? originalTransaction.ExpenseCategoryId;
            originalTransaction.IncomeCategoryId = updatedTransaction.IncomeCategoryId ?? originalTransaction.IncomeCategoryId;
            originalTransaction.Amount = updatedTransaction.Amount == 0 ? originalTransaction.Amount : updatedTransaction.Amount;

            await _repositoryBase.UpdateItemAsync(id, originalTransaction);

            if (fromAccountId.HasValue)
            {
                await UpdateAccountBalance(fromAccountId.Value, -amount);
            }
            if (toAccountId.HasValue)
            {
                await UpdateAccountBalance(toAccountId.Value, amount);
            }

            await _repositoryBase.CommitAsync();
        }
        catch (Exception)
        {
            _repositoryBase.Rollback();
            throw;
        }
    }

    public async Task DeleteTransaction(int id, string username)
    {
        var transaction = await GetTransactionForUser(id, username);

        try
        {
            await _repositoryBase.BeginTransactionAsync();

            if (transaction.FromAccountId.HasValue)
            {
                await UpdateAccountBalance(transaction.FromAccountId.Value, transaction.Amount);
            }
            if (transaction.ToAccountId.HasValue)
            {
                await UpdateAccountBalance(transaction.ToAccountId.Value, -transaction.Amount);
            }

            await _repositoryBase.DeleteItemAsync(transaction);

            await _repositoryBase.CommitAsync();
        }
        catch (Exception)
        {
            _repositoryBase.Rollback();
            throw;
        }
    }

    // private methods
    private async Task UpdateAccountBalance(int accountId, decimal amount)
    {
        var account = await _accountRepository.GetItemAsync(accountId);
        if (account == null)
            throw new Exception("Account not found");

        account.Balance += amount;

        await _accountRepository.UpdateItemAsync(accountId, account);
    }

    private async Task<Transaction> GetTransactionForUser(int id, string username)
    {
        var transaction = await _repositoryBase.GetItemAsync(id);
        if (transaction == null)
            throw new Exception("Transaction not found");

        await ValidateUserAccountAccess(transaction.FromAccountId, username);
        await ValidateUserAccountAccess(transaction.ToAccountId, username);

        return transaction;
    }

    private async Task<Dictionary<int, GetCategoryDtoShort>> FetchCategoriesForTransactions(IEnumerable<Transaction> transactions)
    {
        if (!transactions.Any())
            return new Dictionary<int, GetCategoryDtoShort>();

        var categoryIds = transactions
            .SelectMany(t => new int?[] { t.ExpenseCategoryId, t.IncomeCategoryId })
            .Distinct()
            .Where(id => id.HasValue)
            .Select(id => id.Value);

        var categories = await _categoryRepository.FindByConditionAsync(c => categoryIds.Contains(c.Id));

        return categories.ToDictionary(c => c.Id, MapToCategoryDto);
    }

    private async Task ValidateTransactionData(AddTransactionDto newTransaction, int? fromAccountId, int? toAccountId, decimal amount)
    {
        if (!fromAccountId.HasValue && !toAccountId.HasValue)
            throw new Exception("There must be at least one account ID in transaction (FromAccountId or ToAccountId)");

        if (amount <= 0)
            throw new Exception("Amount must be greater than 0");

        if (fromAccountId.HasValue)
            if (!newTransaction.ExpenseCategoryId.HasValue)
                throw new Exception("ExpenseCategoryId must be specified if there is FromAccountId");

        if (toAccountId.HasValue)
            if (!newTransaction.IncomeCategoryId.HasValue)
                throw new Exception("IncomeCategoryId must be specified if there is ToAccountId");

        await ValidateCategoriesData(newTransaction);

        if (fromAccountId == toAccountId)
            throw new Exception("FromAccountId and ToAccountId can not be the same");
    }

    private async Task ValidateCategoriesData(AddTransactionDto newTransaction)
    {
        if (newTransaction.ExpenseCategoryId.HasValue)
        {
            var category = await _categoryRepository.GetItemAsync(newTransaction.ExpenseCategoryId.Value);

            if (category == null)
                throw new Exception("Category not found");

            if (category.Type != "expense")
                throw new Exception("ExpenseCategoryId must be id of category with type 'expense'");
        }

        if (newTransaction.IncomeCategoryId.HasValue)
        {
            var category = await _categoryRepository.GetItemAsync(newTransaction.IncomeCategoryId.Value);

            if (category == null)
                throw new Exception("Category not found");

            if (category.Type != "income")
                throw new Exception("IncomeCategoryId must be id of category with type 'income'");
        }
    }

    private async Task ValidateUserAccountAccess(int? accountId, string username)
    {
        if (accountId.HasValue)
        {
            var user = await GetUserByUsername(username);
            var account = await _accountRepository.GetItemAsync(accountId.Value);
            if (account == null)
                throw new Exception("Account not found");
            if (account.UserId != user.Id)
                throw new Exception($"Unauthorized access to the account");
        }
    }

    private async Task<User> GetUserByUsername(string username)
    {
        var user = await _userRepository.FindByNameAsync(username);
        if (user == null)
            throw new Exception("User not found");
        return user;
    }

    private GetTransactionDto MapToTransactionDto(Transaction transaction, Dictionary<int, GetCategoryDtoShort> categoryMap)
    {
        return new GetTransactionDto
        {
            Id = transaction.Id,
            FromAccountId = transaction.FromAccountId,
            ToAccountId = transaction.ToAccountId,
            ExpenseCategory = transaction.ExpenseCategoryId.HasValue && categoryMap.ContainsKey(transaction.ExpenseCategoryId.Value) ? categoryMap[transaction.ExpenseCategoryId.Value] : null,
            IncomeCategory = transaction.IncomeCategoryId.HasValue && categoryMap.ContainsKey(transaction.IncomeCategoryId.Value) ? categoryMap[transaction.IncomeCategoryId.Value] : null,
            Amount = transaction.Amount,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt,
        };
    }

    private GetCategoryDtoShort MapToCategoryDto(Category category)
    {
        return new GetCategoryDtoShort
        {
            Id = category.Id,
            Name = category.Name,
        };
    }
}
