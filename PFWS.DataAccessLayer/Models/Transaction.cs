namespace PFWS.DataAccessLayer.Models;

public class Transaction
{
    public int Id { get; set; }
    public int FromAccountId { get; set; }
    public Account FromAccount { get; set; }
    public int ToAccountId { get; set; }
    public Account ToAccount { get; set; }
    public int ExpenseCategoryId { get; set; }
    public Category ExpenseCategory { get; set; }
    public int IncomeCategoryId { get; set; }
    public Category IncomeCategory { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}
