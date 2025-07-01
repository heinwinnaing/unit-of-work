using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace UnitOfWork;

public interface IRepository<T> where T : class
{
    IQueryable<T> Query(Expression<Func<T, bool>>? expression = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? order = null);

    IEnumerable<T> GetAll(Expression<Func<T, bool>>? expression = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? order = null);
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? expression = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? order = null,
        CancellationToken cancellationToken = default);

    T? Get(object id);
    T? Get(Expression<Func<T, bool>> expression);
    Task<T?> GetAsync(object id, CancellationToken cancellationToken = default);
    Task<T?> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);

    bool Any(Expression<Func<T, bool>> expression);
    Task<bool> AnyAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);

    TValue? Max<TValue>(Expression<Func<T, bool>> expression, Expression<Func<T, TValue>> field);
    Task<TValue?> MaxAsync<TValue>(Expression<Func<T, bool>> expression, Expression<Func<T, TValue>> field, CancellationToken cancellationToken = default);

    void Refresh(T entity);
    Task RefreshAsync(T entity, CancellationToken cancellationToken = default);

    T Add(T entity);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    void Add(IEnumerable<T> entities);

    void Update(T entity);
    void Update(IEnumerable<T> entities);

    void Delete(T entity);
    void Delete(IEnumerable<T> entities);
}
