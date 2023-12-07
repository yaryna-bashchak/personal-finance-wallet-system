using PFWS.BusinessLogicLayer.DTOs.Account;

namespace PFWS.BusinessLogicLayer.Services;

public interface IAccountService
{
    public Task<GetAccountDto> GetAccountById(int id, string username);
    public Task<List<GetAccountDto>> GetUserAccounts(string username);
    public Task AddAccount(AddAccountDto newAccount, string username);
    public Task UpdateAccount(int id, UpdateAccountDto updatedAccount, string username);
    public Task DeleteAccount(int id, string username);
}
