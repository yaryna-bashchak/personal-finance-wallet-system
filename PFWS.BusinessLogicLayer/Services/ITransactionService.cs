using PFWS.BusinessLogicLayer.DTOs.Transactions;

namespace PFWS.BusinessLogicLayer.Services;

public interface ITransactionService
{
    public Task<List<GetTransactionDto>> GetTransactionsByAccountId(int accountId, string username);
    public Task<GetTransactionDto> GetTransactionById(int id, string username);
    public Task AddTransaction(AddTransactionDto newTransaction, string username);
    public Task UpdateTransaction(int id, UpdateTransactionDto updatedTransaction, string username);
    public Task DeleteTransaction(int id, string username);
}
