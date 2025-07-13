namespace Dynamiq.API.Extension.Interfaces
{
    public interface ICrudRepo<T> where T : class
    {
        Task<List<T>> GetAll();
        Task<T> GetById(Guid id);
        Task Delete(Guid id);
        Task<T> Update(T entity);
        Task<T> Insert(T entity);
    }
}
