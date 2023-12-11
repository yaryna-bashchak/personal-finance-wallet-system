using System.Collections;

namespace PFWS.DataAccessLayer.Tests;

public class AccountComparer : IComparer
{
    public int Compare(object? x, object? y)
    {
        var lhs = x as Account;
        var rhs = y as Account;
        if (lhs == null || rhs == null)
        {
            throw new InvalidOperationException("Object is not an Account");
        }

        if (lhs.Id != rhs.Id || lhs.Name != rhs.Name || lhs.Balance != rhs.Balance || lhs.UserId != rhs.UserId)
        {
            return -1;
        }
        return 0;
    }

    public bool Equals(Account x, Account y)
    {
        if (x == null || y == null)
        {
            return false;
        }

        return x.Id == y.Id &&
               x.Name == y.Name &&
               x.Balance == y.Balance &&
               x.UserId == y.UserId;
    }
}
