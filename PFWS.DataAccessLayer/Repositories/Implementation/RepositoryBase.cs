
using Microsoft.EntityFrameworkCore;
using PFWS.DataAccessLayer.Data;
using PFWS.DataAccessLayer.Models;

namespace PFWS.DataAccessLayer.Repositories.Implementation;

public class RepositoryBase<T> : IRepositoryBase<T> where T : EntityBase
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

    public async Task<T> GetItem(int id)
    {
        return await _context.Set<T>().FirstAsync(s => s.Id == id);
    }

    public async Task AddItem(T newItem)
    {
        try
        {
            newItem.Id = _context.Set<T>().Max(s => s.Id) + 1;
            await _context.Set<T>().AddAsync(newItem);
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task UpdateItem(int id, T updatedItem)
    {
        var item = await _context.Set<T>().FirstOrDefaultAsync(s => s.Id == id);
        if (item == null)
        {
            throw new KeyNotFoundException("Item not found.");
        }

        _context.Entry(item).CurrentValues.SetValues(updatedItem);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteItem(int id)
    {
        try
        {
            var item = await _context.Set<T>().FirstOrDefaultAsync(s => s.Id == id);

            if (item == null)
            {
                throw new KeyNotFoundException($"Item with ID {id} not found.");
            }

            _context.Set<T>().Remove(item);
            await _context.SaveChangesAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }
}
