using SharedLibrary.Responses;

namespace SharedLibrary.Repositories;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> ListAllAsync();
    Task<IEnumerable<T>> ListAsync(ISpecification<T> spec);
    Task<T> GetBySpecAsync(ISpecification<T> spec);
    Task<int> CountAsync(ISpecification<T> spec);
    Task<bool> AnyAsync(ISpecification<T> spec);
    Task<Response> AddAsync(T entity);
    Task<Response> UpdateAsync(T entity);
    Task<Response> DeleteAsync(T entity);
    Task<Response> AddRangeAsync(IEnumerable<T> entities);
    Task<Response> UpdateRangeAsync(IEnumerable<T> entities);
    Task<Response> DeleteRangeAsync(IEnumerable<T> entities);
}
