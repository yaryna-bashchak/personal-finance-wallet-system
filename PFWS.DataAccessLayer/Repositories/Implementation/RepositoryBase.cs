
using Microsoft.EntityFrameworkCore;
using PFWS.DataAccessLayer.Data;

namespace PFWS.DataAccessLayer.Repositories.Implementation;

public class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    private readonly WalletContext _context;
    public RepositoryBase(WalletContext context)
    {
        _context = context;
    }
    public async Task<List<T>> GetAllItems()
    {
        return await _context.Set<T>().ToListAsync();
    }
}
