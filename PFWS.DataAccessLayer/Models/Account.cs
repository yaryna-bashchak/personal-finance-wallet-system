namespace PFWS.DataAccessLayer.Models;

public class Account
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Balance { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public List<Transaction> TransactionsTo { get; set; }
    public List<Transaction> TransactionsFrom { get; set; }
}
