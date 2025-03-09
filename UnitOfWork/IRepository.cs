using System.Linq.Expressions;

namespace UnitOfWork;

public interface IRepository<T> where T : class
{
    IQueryable<T> Query(Expression<Func<T, bool>> expression);

    T? Get(Expression<Func<T, bool>> expression);
    Task<T?> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);

    T Add(T entity);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    void Add(IEnumerable<T> entities);

    void Update(T entity);
    void Update(IEnumerable<T> entities);

    void Delete(T entity);
    void Delete(IEnumerable<T> entities);
}
