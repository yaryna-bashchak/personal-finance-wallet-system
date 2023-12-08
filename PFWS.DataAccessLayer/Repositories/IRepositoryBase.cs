using System.Linq.Expressions;

namespace PFWS.DataAccessLayer.Repositories;

public interface IRepositoryBase<T>
{
    public Task<List<T>> GetAllItems();
    public Task<T> GetItem(int id);
    public Task AddItem(T newItem);
    public Task UpdateItem(int id, T updatedItem);
    public Task DeleteItem(T item);
    public Task<List<T>> FindByCondition(Expression<Func<T, bool>> expression);
}
