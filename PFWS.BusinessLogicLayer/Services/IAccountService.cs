using PFWS.BusinessLogicLayer.DTOs.Account;

namespace PFWS.BusinessLogicLayer.Services;

public interface IAccountService
{
    public Task<GetAccountDto> GetAccountById(int id);
    public Task AddAccount(AddAccountDto newAccount);
    public Task UpdateAccount(int id, UpdateAccountDto updatedAccount);
    public Task DeleteAccount(int id);
}
