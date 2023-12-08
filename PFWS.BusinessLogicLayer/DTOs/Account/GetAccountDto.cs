namespace PFWS.BusinessLogicLayer.DTOs.Account;

public class GetAccountDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
