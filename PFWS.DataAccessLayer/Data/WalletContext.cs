using Microsoft.EntityFrameworkCore;
using PFWS.DataAccessLayer.Models;

namespace PFWS.DataAccessLayer.Data;

public class WalletContext : DbContext
{
    public WalletContext(DbContextOptions options) : base(options)
    {      
    }

    public DbSet<Account> Accounts { get; set; }
}
