using System.Linq.Expressions;

namespace BookStore.DataAccess.Repositories.IRepositories
{
    public interface IRepository<T> where T : class                       // Generic repository interface for all type of class
    {
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

        Task<T?> GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null);

        Task<bool> Add(T entity);

        bool Remove(T entity);

        bool RemoveRange(IEnumerable<T> entities);
    }
}
