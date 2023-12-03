using PFWS.DataAccessLayer.Models;

namespace PFWS.BusinessLogicLayer.Services;

public interface IAccountService
{
    public Task<List<Account>> GetAllAccounts();
}
