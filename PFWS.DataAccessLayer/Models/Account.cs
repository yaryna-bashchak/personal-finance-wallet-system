namespace PFWS.DataAccessLayer.Models;

public class Account : EntityBase
{
    public string Name { get; set; }
    public decimal Balance { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public List<Transaction> TransactionsTo { get; set; }
    public List<Transaction> TransactionsFrom { get; set; }

    public Account() : base()
    {

    }
}
