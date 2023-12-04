using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PFWS.DataAccessLayer.Models;

namespace PFWS.DataAccessLayer.Data;

public class WalletContext : IdentityDbContext<User>
{
    public WalletContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(c => c.Accounts)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Account>()
            .HasMany(a => a.TransactionsTo)
            .WithOne(t => t.FromAccount)
            .HasForeignKey(t => t.FromAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Account>()
            .HasMany(a => a.TransactionsFrom)
            .WithOne(t => t.ToAccount)
            .HasForeignKey(t => t.ToAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.ExpenseCategory)
            .WithMany(c => c.ExpenseTransactions)
            .HasForeignKey(t => t.ExpenseCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.IncomeCategory)
            .WithMany(c => c.IncomeTransactions)
            .HasForeignKey(t => t.IncomeCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Category> Categories { get; set; }
}
