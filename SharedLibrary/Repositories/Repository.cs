using Microsoft.EntityFrameworkCore;
using SharedLibrary.Responses;

namespace SharedLibrary.Repositories;

public class Repository<T>(DbContext context) : IRepository<T> where T : class
{
    protected readonly DbContext _context = context;
    protected readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> ListAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<T>> ListAsync(ISpecification<T> spec)
    {
        var query = SpecificationEvaluator<T>.GetQuery(_dbSet, spec);
        return await query.ToListAsync();
    }

    public async Task<T> GetBySpecAsync(ISpecification<T> spec)
    {
        var query = SpecificationEvaluator<T>.GetQuery(_dbSet, spec);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<int> CountAsync(ISpecification<T> spec)
    {
        var query = SpecificationEvaluator<T>.GetQuery(_dbSet, spec);
        return await query.CountAsync();
    }

    public async Task<bool> AnyAsync(ISpecification<T> spec)
    {
        var query = SpecificationEvaluator<T>.GetQuery(_dbSet, spec);
        return await query.AnyAsync();
    }

    public async Task<Response> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return new Response(true, "Entity added successfully");
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

    public async Task<Response> AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
        return new Response(true, "Entities added successfully");
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
