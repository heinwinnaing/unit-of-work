using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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

    public IQueryable<T> Query(Expression<Func<T, bool>>? expression = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? order = null)
    {
        IQueryable<T> query = _dbSet;
        if (include is not null) query = include(query);
        if (expression is not null) query = query.Where(expression);
        if (order is not null) query = order(query);

        return query;
    }

    public IEnumerable<T> GetAll(Expression<Func<T, bool>>? expression = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? order = null)
    {
        IQueryable<T> query = this.Query(
            expression: expression,
            include: include,
            order: order);

        return query.ToList();
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? expression = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, Func<IQueryable<T>, IOrderedQueryable<T>>? order = null, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = this.Query(
            expression: expression,
            include: include,
            order: order);

        return await query.ToListAsync(cancellationToken);
    }

    public T? Get(object id)
    {
        return _dbSet.Find(id);
    }

    public T? Get(Expression<Func<T, bool>> expression)
    {
        return _dbSet.FirstOrDefault(expression);
    }

    public async Task<T?> GetAsync(object id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(id, cancellationToken);
    }
    public async Task<T?> GetAsync(Expression<Func<T, bool>> expression,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(expression, cancellationToken);
    }

    public bool Any(Expression<Func<T, bool>> expression)
    {
        return _dbSet.Any(expression);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(expression, cancellationToken);
    }

    public TValue? Max<TValue>(Expression<Func<T, bool>> expression, Expression<Func<T, TValue>> field)
    {
        return _dbSet.Where(expression).Max(field);
    }

    public async Task<TValue?> MaxAsync<TValue>(Expression<Func<T, bool>> expression, Expression<Func<T, TValue>> field, CancellationToken cancellationToken = default)
    {
        return await _dbSet
                .Where(expression)
                .MaxAsync(field, cancellationToken);
    }

    public void Refresh(T entity)
    {
        _dbContext.Entry<T>(entity).Reload();
    }

    public async Task RefreshAsync(T entity, 
        CancellationToken cancellationToken = default)
    {
        await _dbContext
            .Entry<T>(entity)
            .ReloadAsync(cancellationToken);
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

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Update(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void Delete(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }
}
