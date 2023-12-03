using PFWS.DataAccessLayer.Models;

namespace PFWS.DataAccessLayer.Data;

public static class DbInitializer
{
    public static void Initialize(WalletContext context)
    {
        if(context.Accounts.Any()) return;

        var accounts = new List<Account>
        {
            new Account
            {
                Name = "Monobank account",
                Balance = 0,
            },
            new Account
            {
                Name = "Privat account",
                Balance = 100,
            },
            new Account
            {
                Name = "Credit Agricole account",
                Balance = 20,
            },
        };

        foreach (var account in accounts)
        {
            context.Accounts.Add(account);
        }

        context.SaveChanges();
    }
}
