using Microsoft.EntityFrameworkCore;

namespace UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public interface IUnitOfWork<TWriter, TReader>
    : IUnitOfWork where TWriter : DbContext where TReader : DbContext
{
    IRepository<TEntity> ReaderRepository<TEntity>() where TEntity : class;
}