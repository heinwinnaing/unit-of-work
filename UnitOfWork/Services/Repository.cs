using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace UnitOfWork.Services;

internal class Repository<T>
    : IRepository<T> where T : class
{
    protected readonly DbSet<T> _dbSet;
    protected readonly DbContext _dbContext;
    public Repository(DbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _dbContext.Set<T>();
    }
    public T Add(T entity)
    {
        var result = _dbSet.Add(entity);
        return result.Entity;
    }

    public void Add(IEnumerable<T> entities)
    {
        _dbSet.AddRange(entities);
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var result = await _dbSet.AddAsync(entity, cancellationToken);
        return result.Entity;
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void Delete(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public T? Get(Expression<Func<T, bool>> expression)
    {
        return _dbSet.FirstOrDefault(expression);
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(expression, cancellationToken);
    }

    public IQueryable<T> Query(Expression<Func<T, bool>> expression)
    {
        return _dbSet.Where(expression);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Update(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }
}
