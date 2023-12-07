namespace PFWS.DataAccessLayer.Models;

public class Category : EntityBase
{
    public string Name { get; set; }
    public string Type { get; set; }
    public List<Transaction> ExpenseTransactions { get; set; }
    public List<Transaction> IncomeTransactions { get; set; }

    public Category() : base()
    {
        
    }
}
