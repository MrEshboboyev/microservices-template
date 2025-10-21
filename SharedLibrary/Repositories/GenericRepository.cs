using Microsoft.EntityFrameworkCore;
using SharedLibrary.Interface;
using SharedLibrary.Responses;
using System.Linq.Expressions;

namespace SharedLibrary.Repositories;

public class GenericRepository<T>(DbContext context) : IGenericInterface<T> where T : class
{
    protected readonly DbContext _context = context;
    protected readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<Response> CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return new Response(true, "Entity created successfully");
    }

    public async Task<Response> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return new Response(true, "Entity updated successfully");
    }

    public async Task<Response> DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return new Response(true, "Entity deleted successfully");
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T> FindByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T> GetByAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, object>> orderBy, bool ascending = true)
    {
        if (ascending)
        {
            return await _dbSet.OrderBy(orderBy).ToListAsync();
        }
        else
        {
            return await _dbSet.OrderByDescending(orderBy).ToListAsync();
        }
    }

    public async Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>> predicate = null)
    {
        var query = predicate != null ? _dbSet.Where(predicate) : _dbSet;
        return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
    {
        if (predicate != null)
        {
            return await _dbSet.CountAsync(predicate);
        }
        else
        {
            return await _dbSet.CountAsync();
        }
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task<Response> CreateRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
        return new Response(true, "Entities created successfully");
    }

    public async Task<Response> UpdateRangeAsync(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
        await _context.SaveChangesAsync();
        return new Response(true, "Entities updated successfully");
    }

    public async Task<Response> DeleteRangeAsync(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
        await _context.SaveChangesAsync();
        return new Response(true, "Entities deleted successfully");
    }
}
