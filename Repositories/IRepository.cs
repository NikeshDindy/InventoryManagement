using InventoryManagement.Models;
using System.Linq.Expressions;

namespace InventoryManagement.Repositories
{
    public interface IRepository<T> where T:class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);

        Task<T?> GetByIdAsync(int id);

        Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        void Update(T entity);

        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

       
    }
}
