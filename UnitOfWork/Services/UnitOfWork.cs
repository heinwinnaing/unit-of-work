using Google.Protobuf;
using Microsoft.EntityFrameworkCore;

namespace UnitOfWork.Services;

internal class UnitOfWork<T> 
    : IDisposable, IUnitOfWork where T : DbContext
{
    protected bool _disposed = false;
    protected Dictionary<Type, object>? _repositories;
    private T dbContext { get; }
    public UnitOfWork(T context)
    {
        dbContext = context ?? throw new ArgumentNullException(nameof(context));
    }
    public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        if (_repositories == null)
            _repositories = new Dictionary<Type, object>();

        var type = typeof(TEntity);
        if (!_repositories.ContainsKey(type))
            _repositories[type] = new Repository<TEntity>(dbContext);

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
                dbContext?.Dispose();
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

internal class UnitOfWork<TWriter, TReader>
    : UnitOfWork<TWriter>, IUnitOfWork<TWriter, TReader> 
    where TWriter : DbContext where TReader : DbContext
{
    public TReader ReaderContext { get; }
    
    public UnitOfWork(TWriter writerContext, TReader readerContext)
        : base(writerContext)
    {
        _= writerContext ?? throw new ArgumentNullException(nameof(writerContext));
        ReaderContext = readerContext ?? throw new ArgumentNullException(nameof(readerContext));
    }

    public IRepository<TEntity> ReaderRepository<TEntity>() where TEntity : class
    {
        if (_repositories == null)
            _repositories = new Dictionary<Type, object>();

        var type = typeof(TEntity);
        if (!_repositories.ContainsKey(type))
            _repositories[type] = new Repository<TEntity>(ReaderContext);

        return (IRepository<TEntity>)_repositories[type];
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                ReaderContext?.Dispose();
                base.Dispose(disposing);
            }
        }
        _disposed = true;
    }
}
