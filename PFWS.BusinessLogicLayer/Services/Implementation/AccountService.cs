using PFWS.DataAccessLayer.Models;
using PFWS.DataAccessLayer.Repositories;

namespace PFWS.BusinessLogicLayer.Services.Implementation;

public class AccountService : IAccountService
{
    private readonly IRepositoryBase<Account> _repositoryBase;
    public AccountService(IRepositoryBase<Account> repositoryBase)
    {
        _repositoryBase = repositoryBase;

    }

    public async Task<List<Account>> GetAllAccounts()
    {
        return await _repositoryBase.GetAllItems();
    }
}
