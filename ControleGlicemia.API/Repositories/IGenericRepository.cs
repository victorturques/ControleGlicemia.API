using System.Linq.Expressions;
using ControleGlicemia.API.Models;

namespace ControleGlicemia.API.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task DeleteAsync(T entity);
    Task UpdateAsync(T entity);
    Task<IEnumerable<T>> GetAllByUserIdAsync(int userId);
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
    Task<PagedResult<T>> GetPagedByUserIdAsync(int userId, int page, int pageSize);
}
