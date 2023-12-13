using PFWS.BusinessLogicLayer.DTOs.Account;
using PFWS.DataAccessLayer.Models;
using PFWS.DataAccessLayer.Repositories;

namespace PFWS.BusinessLogicLayer.Services.Implementation;

public class AccountService : IAccountService
{
    private readonly IRepositoryBase<Account> _repositoryBase;
    private readonly IUserRepository _userRepository;

    public AccountService(IRepositoryBase<Account> repositoryBase, IUserRepository userRepository)
    {
        _repositoryBase = repositoryBase;
        _userRepository = userRepository;
    }

    public async Task AddAccount(AddAccountDto newAccount, string username)
    {
        var user = await GetUserByUsername(username);

        var account = new Account
        {
            UserId = user.Id,
            Name = newAccount.Name,
            Balance = newAccount.Balance,
        };

        await _repositoryBase.AddItemAsync(account);
    }

    public async Task DeleteAccount(int id, string username)
    {
        var user = await GetUserByUsername(username);
        var account = await GetAccountById(id, user.Id);

        await _repositoryBase.DeleteItemAsync(account);
    }

    public async Task<GetAccountDto> GetAccountById(int id, string username)
    {
        var user = await GetUserByUsername(username);
        var account = await GetAccountById(id, user.Id);

        var accountDto = MapToAccountDto(account);

        return accountDto;
    }

    public async Task<List<GetAccountDto>> GetUserAccounts(string username)
    {
        var user = await GetUserByUsername(username);
        var userAccounts = await _repositoryBase.FindByConditionAsync(account => account.UserId == user.Id);

        var userAccountsDto = userAccounts.Select(MapToAccountDto).ToList();

        return userAccountsDto;
    }

    public async Task UpdateAccount(int id, UpdateAccountDto updatedAccount, string username)
    {
        var user = await GetUserByUsername(username);
        var account = await GetAccountById(id, user.Id);

        account.Name = updatedAccount.Name;

        await _repositoryBase.UpdateItemAsync(id, account);
    }

    private async Task<User> GetUserByUsername(string username)
    {
        var user = await _userRepository.FindByNameAsync(username);
        if (user == null)
            throw new Exception("User not found");
        return user;
    }

    private async Task<Account> GetAccountById(int id, int userId)
    {
        var account = await _repositoryBase.GetItemAsync(id);
        if (account == null)
            throw new Exception("Account not found");
        if (account.UserId != userId)
            throw new Exception("Unauthorized access to the account");

        return account;
    }

    private GetAccountDto MapToAccountDto(Account account)
    {
        return new GetAccountDto
        {
            Id = account.Id,
            UserId = account.UserId,
            Name = account.Name,
            Balance = account.Balance,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt,
        };
    }
}
