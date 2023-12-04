using Microsoft.AspNetCore.Identity;
using PFWS.DataAccessLayer.Models;

namespace PFWS.DataAccessLayer.Data;

public static class DbInitializer
{
    public static async Task Initialize(WalletContext context, UserManager<User> userManager)
    {
        await InitializeUsers(userManager);
        InitializeAccounts(context);
        InitializeCategories(context);
        InitializeTransactions(context);
    }

    private static void InitializeTransactions(WalletContext context)
    {
        if (context.Transactions.Any()) return;

        var transactions = new List<Transaction>
        {
            new Transaction
            {
                ToAccountId = 1,
                IncomeCategoryId = 4,
                Amount = 1000,
                CreatedAt = new DateTime(2023, 12, 2, 16, 0, 0),
                UpdatedAt = new DateTime(2023, 12, 2, 16, 0, 0),
            },
            new Transaction
            {
                ToAccountId = 1,
                IncomeCategoryId = 4,
                Amount = 1500,
                CreatedAt = new DateTime(2023, 12, 3, 16, 0, 0),
                UpdatedAt = new DateTime(2023, 12, 3, 16, 0, 0),
            },
            new Transaction
            {
                ToAccountId = 1,
                IncomeCategoryId = 5,
                Amount = 200,
            },
            new Transaction
            {
                FromAccountId = 1,
                ToAccountId = 2,
                ExpenseCategoryId = 6,
                IncomeCategoryId = 7,
                Amount = 1200,
            },
            new Transaction
            {
                FromAccountId = 1,
                ToAccountId = 2,
                ExpenseCategoryId = 6,
                IncomeCategoryId = 7,
                Amount = 50,
            },
            new Transaction
            {
                FromAccountId = 1,
                ExpenseCategoryId = 1,
                Amount = 100,
                CreatedAt = new DateTime(2023, 12, 3, 10, 0, 0),
                UpdatedAt = new DateTime(2023, 12, 3, 10, 0, 0),
            },
            new Transaction
            {
                FromAccountId = 1,
                ExpenseCategoryId = 1,
                Amount = 50,
                CreatedAt = new DateTime(2023, 12, 3, 10, 30, 0),
                UpdatedAt = new DateTime(2023, 12, 3, 10, 30, 0),
            },
            new Transaction
            {
                FromAccountId = 1,
                ExpenseCategoryId = 1,
                Amount = 20,
                CreatedAt = new DateTime(2023, 12, 3, 12, 0, 0),
                UpdatedAt = new DateTime(2023, 12, 3, 12, 0, 0),
            },
            new Transaction
            {
                FromAccountId = 1,
                ExpenseCategoryId = 2,
                Amount = 300,
                CreatedAt = new DateTime(2023, 12, 3, 17, 0, 0),
                UpdatedAt = new DateTime(2023, 12, 3, 17, 0, 0),
            },
            new Transaction
            {
                FromAccountId = 2,
                ExpenseCategoryId = 3,
                Amount = 300,
                CreatedAt = new DateTime(2023, 12, 4, 9, 0, 0),
                UpdatedAt = new DateTime(2023, 12, 4, 9, 0, 0),
            },
        };

        foreach (var transaction in transactions)
        {
            context.Transactions.Add(transaction);
        }

        context.SaveChanges();
    }

    private static void InitializeCategories(WalletContext context)
    {
        if (context.Categories.Any()) return;

        var categories = new List<Category>
        {
            new Category
            {
                Name = "Food",
                Type = "expense",
            },
            new Category
            {
                Name = "Home",
                Type = "expense",
            },
            new Category
            {
                Name = "Clothes",
                Type = "expense",
            },
            new Category
            {
                Name = "Salary",
                Type = "income",
            },
            new Category
            {
                Name = "Interest on deposits",
                Type = "income",
            },
            new Category
            {
                Name = "Transfer to another account",
                Type = "expense",
            },
            new Category
            {
                Name = "Transfer from another account",
                Type = "income",
            },
        };

        foreach (var category in categories)
        {
            context.Categories.Add(category);
        }

        context.SaveChanges();
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
                Balance = 1500,
                UserId = userYaryna.Id,
                CreatedAt = new DateTime(2023, 12, 1, 9, 30, 0),
                UpdatedAt = new DateTime(2023, 12, 1, 9, 30, 0),
            },
            new Account
            {
                Name = "Privat account",
                Balance = 2100,
                UserId = userYaryna.Id,
            },
            new Account
            {
                Name = "Credit Agricole account",
                Balance = 100,
                UserId = userYaryna.Id,
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
                Name = "Monobank account 1",
                Balance = 2000,
                UserId = userSam.Id,
                CreatedAt = new DateTime(2023, 12, 2, 13, 30, 0),
                UpdatedAt = new DateTime(2023, 12, 2, 13, 30, 0),
            },
            new Account
            {
                Name = "Monobank account 2",
                Balance = 700,
                UserId = userSam.Id,
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
