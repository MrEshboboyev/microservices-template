using SharedLibrary.Responses;
using System.Linq.Expressions;

namespace SharedLibrary.Interface;

public interface IGenericInterface<T> where T : class
{
    Task<Response> CreateAsync(T entity);
    Task<Response> UpdateAsync(T entity);
    Task<Response> DeleteAsync(T entity);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> FindByIdAsync(int id);
    Task<T> GetByAsync(Expression<Func<T, bool>> predicate);
    
    // Enhanced methods
    Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, object>> orderBy, bool ascending = true);
    Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>> predicate = null);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<Response> CreateRangeAsync(IEnumerable<T> entities);
    Task<Response> UpdateRangeAsync(IEnumerable<T> entities);
    Task<Response> DeleteRangeAsync(IEnumerable<T> entities);
}
