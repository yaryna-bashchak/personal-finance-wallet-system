using Microsoft.AspNetCore.Identity;

namespace PFWS.DataAccessLayer.Models;

public class User : IdentityUser<int>, IEntityBaseOrIdentityUser
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<Account> Accounts { get; set; }

    public User()
    {
        var date = DateTime.Now;
        CreatedAt = date;
        UpdatedAt = date;
    }
}
