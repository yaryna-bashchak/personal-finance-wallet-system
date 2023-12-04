using Microsoft.AspNetCore.Identity;

namespace PFWS.DataAccessLayer.Models;

public class User : IdentityUser
{
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public List<Account> Accounts { get; set; }
}