using System.Linq.Expressions;
using ControleGlicemia.API.Data;
using ControleGlicemia.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleGlicemia.API.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id)
        => await _dbSet.FindAsync(id);

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        if (entity is ISoftDeletable softDeletable)
        {
            softDeletable.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        else
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<T>> GetAllByUserIdAsync(int userId)
    {
        var propriedade = typeof(T) == typeof(User) ? "Id" : "UserId";
        return await _dbSet.Where(e => EF.Property<int>(e, propriedade) == userId).ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
    {
        if (filter is null)
            return await _dbSet.ToListAsync();

        return await _dbSet.Where(filter).ToListAsync();
    }

    public async Task<PagedResult<T>> GetPagedByUserIdAsync(int userId, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var propriedade = typeof(T) == typeof(User) ? "Id" : "UserId";
        var query = _dbSet.Where(e => EF.Property<int>(e, propriedade) == userId);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
