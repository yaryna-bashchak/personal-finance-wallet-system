namespace PFWS.DataAccessLayer.Repositories;

public interface IRepositoryBase<T>
{
    Task<List<T>> GetAllItems();
}
