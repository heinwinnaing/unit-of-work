using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace UnitOfWork.Extensions;

public interface IUnitOfWorkOption
{
    IServiceCollection UseMySql<T>(string connectionString) where T : DbContext;
    IServiceCollection UseMySql<TWriter, TReader>(string wirterConnectionString,
        string readerConnectionString)
        where TWriter : DbContext
        where TReader : DbContext;
}
