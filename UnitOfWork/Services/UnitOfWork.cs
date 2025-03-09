
using Microsoft.EntityFrameworkCore;

namespace UnitOfWork.Services;

internal class UnitOfWork<T> : IDisposable, IUnitOfWork where T : DbContext
{
    private bool _disposed = false;
    private T dbContext { get; }
    private Dictionary<Type, object> _repositories;
    public UnitOfWork(T context)
    {
        dbContext = context ?? throw new ArgumentNullException(nameof(context));
        _repositories = new Dictionary<Type, object>();
    }
    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        if (_repositories == null)
            _repositories = new Dictionary<Type, object>();

        var type = typeof(TEntity);
        if (!_repositories.ContainsKey(type)) _repositories[type] = new Repository<TEntity>(dbContext);
        return (IRepository<TEntity>)_repositories[type];
    }

    public int SaveChanges()
    {
        return dbContext.SaveChanges();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.SaveChangesAsync(cancellationToken);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                dbContext.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
