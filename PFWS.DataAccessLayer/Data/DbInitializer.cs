using Microsoft.AspNetCore.Identity;
using PFWS.DataAccessLayer.Models;

namespace PFWS.DataAccessLayer.Data;

public static class DbInitializer
{
    public static async Task Initialize(WalletContext context, UserManager<User> userManager)
    {
        await InitializeUsers(userManager);
        InitializeAccounts(context);
    }

    private static void InitializeAccounts(WalletContext context)
    {
        if (context.Accounts.Any()) return;

        var userYaryna = context.Users.FirstOrDefault(user => user.UserName == "yaryna");
        if (userYaryna == null) return;

        var yarynaAccounts = new List<Account>
        {
            new Account
            {
                Name = "Monobank account",
                Balance = 0,
                UserId = userYaryna.Id,
                CreatedAt = new DateTime(2023, 12, 1, 9, 30, 0),
                UpdatedAt = new DateTime(2023, 12, 1, 9, 30, 0),
            },
            new Account
            {
                Name = "Privat account",
                Balance = 100,
                UserId = userYaryna.Id,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            },
            new Account
            {
                Name = "Credit Agricole account",
                Balance = 20,
                UserId = userYaryna.Id,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            },
        };

        foreach (var account in yarynaAccounts)
        {
            context.Accounts.Add(account);
        }

        var userSam = context.Users.FirstOrDefault(user => user.UserName == "sam");
        if (userSam == null) return;

        var samAccounts = new List<Account>
        {
            new Account
            {
                Name = "Monobank account",
                Balance = 50,
                UserId = userSam.Id,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            },
        };

        foreach (var account in samAccounts)
        {
            context.Accounts.Add(account);
        }

        context.SaveChanges();
    }

    private static async Task InitializeUsers(UserManager<User> userManager)
    {
        if (!userManager.Users.Any())
        {
            var user1 = new User
            {
                UserName = "yaryna",
                CreatedAt = new DateTime(2023, 12, 1, 9, 0, 0),
                UpdatedAt = new DateTime(2023, 12, 1, 9, 0, 0),
            };

            await userManager.CreateAsync(user1, "Yaryna123");

            var user2 = new User
            {
                UserName = "sam",
                CreatedAt = new DateTime(2023, 12, 2, 12, 30, 0),
                UpdatedAt = new DateTime(2023, 12, 2, 12, 30, 0),
            };

            await userManager.CreateAsync(user2, "Sam12345");
        }
    }
}
