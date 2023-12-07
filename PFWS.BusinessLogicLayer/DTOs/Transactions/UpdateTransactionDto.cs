using PFWS.BusinessLogicLayer.DTOs.Category;

namespace PFWS.BusinessLogicLayer.DTOs.Transactions;

public class UpdateTransactionDto
{
    public int? FromAccountId { get; set; }
    public int? ToAccountId { get; set; }
    public int? ExpenseCategoryId { get; set; }
    public int? IncomeCategoryId { get; set; }
    public decimal Amount { get; set; }
}
