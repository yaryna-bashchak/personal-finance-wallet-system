using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PFWS.DataAccessLayer.Data;
using PFWS.DataAccessLayer.Models;

namespace PFWS.DataAccessLayer.Repositories.Implementation;

public class RepositoryBase<T> : IRepositoryBase<T> where T : class, IEntityBaseOrIdentityUser
{
    private readonly WalletContext _context;
    private IDbContextTransaction _currentTransaction;

    public RepositoryBase(WalletContext context)
    {
        _context = context;
    }

    public async Task<List<T>> GetAllItemsAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T> GetItemAsync(int id)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task AddItemAsync(T newItem)
    {
        newItem.Id = _context.Set<T>().Max(s => s.Id) + 1;
        await _context.Set<T>().AddAsync(newItem);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateItemAsync(int id, T updatedItem)
    {
        var item = await _context.Set<T>().FirstOrDefaultAsync(s => s.Id == id);
        if (item == null)
        {
            throw new KeyNotFoundException("Item not found.");
        }

        updatedItem.UpdatedAt = DateTime.Now;
        _context.Entry(item).CurrentValues.SetValues(updatedItem);
        _context.Entry(item).Property("CreatedAt").IsModified = false;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteItemAsync(T item)
    {
        _context.Set<T>().Remove(item);
        await _context.SaveChangesAsync();
    }

    public async Task<List<T>> FindByConditionAsync(Expression<Func<T, bool>> expression)
    {
        return await _context.Set<T>().Where(expression).AsNoTracking().ToListAsync();
    }

    // transactions
    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            _currentTransaction?.Commit();
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void Rollback()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void Dispose()
    {
        if (_currentTransaction != null)
        {
            _currentTransaction.Dispose();
            _currentTransaction = null;
        }

        _context.Dispose();
    }
}
