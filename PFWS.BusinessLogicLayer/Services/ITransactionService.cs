using PFWS.BusinessLogicLayer.DTOs.Transactions;

namespace PFWS.BusinessLogicLayer.Services;

public interface ITransactionService
{
    public Task<List<GetTransactionDto>> GetTransactionsByAccountId(int accountId);
    public Task<GetTransactionDto> GetTransactionById(int id);
    public Task AddTransaction(AddTransactionDto newTransaction);
    public Task UpdateTransaction(int id, UpdateTransactionDto updatedTransaction);
    public Task DeleteTransaction(int id);
}
