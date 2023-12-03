using Microsoft.AspNetCore.Identity;

namespace PFWS.DataAccessLayer.Models;

public class User : IdentityUser
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Account> Accounts { get; set; }
}
