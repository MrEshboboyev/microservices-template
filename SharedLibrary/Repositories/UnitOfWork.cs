using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SharedLibrary.Responses;

namespace SharedLibrary.Repositories;

public class UnitOfWork(
    DbContext context
) : IUnitOfWork
{
    private readonly Dictionary<Type, object> _repositories = [];
    private IDbContextTransaction _transaction;

    public IRepository<T> Repository<T>() where T : class
    {
        if (_repositories.ContainsKey(typeof(T)))
        {
            return (IRepository<T>)_repositories[typeof(T)];
        }

        var repository = new Repository<T>(context);
        _repositories.Add(typeof(T), repository);
        return repository;
    }

    public async Task<int> CompleteAsync()
    {
        return await context.SaveChangesAsync();
    }

    public async Task<Response> SaveChangesAsync()
    {
        try
        {
            await context.SaveChangesAsync();
            return new Response(true, "Changes saved successfully");
        }
        catch (Exception ex)
        {
            return new Response(false, $"Error saving changes: {ex.Message}");
        }
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await CompleteAsync();
            await _transaction.CommitAsync();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        await _transaction.RollbackAsync();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        context?.Dispose();
    }
}
