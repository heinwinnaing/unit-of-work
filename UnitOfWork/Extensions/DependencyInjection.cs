using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UnitOfWork.Services;

namespace UnitOfWork.Extensions;
public class UnitOfWorkOption : IUnitOfWorkOption
{
    private readonly IServiceCollection _services;
    public UnitOfWorkOption(IServiceCollection services)
    {
        _services = services;
    }

    public IServiceCollection UseMySql<T>(string connectionString) where T : DbContext
    {
        _services.AddDbContextPool<T>(options =>
        {
            options.UseMySQL(connectionString);
        })
            .AddScoped<IUnitOfWork, UnitOfWork<T>>();
        return _services;
    }
}

public static class DependencyInjection
{
    public static IServiceCollection AddUnitOfWork(this IServiceCollection services,
        Action<IUnitOfWorkOption> action)
    {
        if (action is null)
            throw new ArgumentNullException(nameof(action));

        var unitOfWorkOption = new UnitOfWorkOption(services);

        action(unitOfWorkOption);

        return services;
    }
}
