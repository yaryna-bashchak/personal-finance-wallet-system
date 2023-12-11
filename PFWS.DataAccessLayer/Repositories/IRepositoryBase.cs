using System.Linq.Expressions;

namespace PFWS.DataAccessLayer.Repositories;

public interface IRepositoryBase<T>
{
    public Task<List<T>> GetAllItemsAsync();
    public Task<T> GetItemAsync(int id);
    public Task AddItemAsync(T newItem);
    public Task UpdateItemAsync(int id, T updatedItem);
    public Task DeleteItemAsync(T item);
    public Task<List<T>> FindByConditionAsync(Expression<Func<T, bool>> expression);
    public Task BeginTransactionAsync();
    public Task CommitAsync();
    public void Rollback();
    public void Dispose();
}
