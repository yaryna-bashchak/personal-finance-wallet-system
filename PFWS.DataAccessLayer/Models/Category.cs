namespace PFWS.DataAccessLayer.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public List<Transaction> ExpenseTransactions { get; set; }
    public List<Transaction> IncomeTransactions { get; set; }
}
