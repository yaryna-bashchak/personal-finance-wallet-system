using PFWS.BusinessLogicLayer.DTOs.Category;

namespace PFWS.BusinessLogicLayer.DTOs.Transactions;

public class GetTransactionDto
{
    public int Id { get; set; }
    public int? FromAccountId { get; set; }
    public int? ToAccountId { get; set; }
    public GetCategoryDto ExpenseCategory { get; set; }
    public GetCategoryDto IncomeCategory { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
