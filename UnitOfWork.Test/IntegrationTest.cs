using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using UnitOfWork.Services;

namespace UnitOfWork.Test;

public record MyTest
{
    [Key]
    public int Id { get; set; }
    public string? Name { get; set; }
}
public class TestDbContext : DbContext
{
    public DbSet<MyTest> MyTests { get; set; }
    public TestDbContext(DbContextOptions options) 
        :base(options) 
    { }
}
public class WriterContext : TestDbContext
{
    public WriterContext(DbContextOptions options) : base(options)
    {
    }
}
public class ReaderContext : TestDbContext
{
    public ReaderContext(DbContextOptions options) : base(options)
    {
    }
}

public class IntegrationTest
{
    private ServiceProvider serviceProvider;
    private readonly IUnitOfWork<WriterContext, ReaderContext> unitOfWork;
    public IntegrationTest()
    {
        var services = new ServiceCollection();
        services.AddDbContextPool<WriterContext>(opts =>
        {
            opts.UseInMemoryDatabase("inMemory_db");
        });
        services.AddDbContextPool<ReaderContext>(opts => 
        {
            opts.UseInMemoryDatabase("inMemory_db");
        });
        services.AddScoped<IUnitOfWork<WriterContext, ReaderContext>, UnitOfWork<WriterContext, ReaderContext>>();
        serviceProvider = services.BuildServiceProvider();

        #region data-initialized
        unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<WriterContext, ReaderContext>>();
        unitOfWork.GetRepository<MyTest>()
            .Add(new MyTest { Id = 1, Name = "Test 1" });
        unitOfWork.GetRepository<MyTest>()
            .Add(new MyTest { Id = 2, Name = "Test 2" });
        unitOfWork.GetRepository<MyTest>()
            .Add(new MyTest { Id = 3, Name = "Test 3" });

        unitOfWork.GetRepository<MyTest>()
            .Add(new MyTest { Id = 100, Name = "test for update" });
        unitOfWork.GetRepository<MyTest>()
            .Add(new MyTest { Id = 101, Name = "test for update" });
        unitOfWork.GetRepository<MyTest>()
            .Add(new MyTest { Id = 200, Name = "test for delete" });
        unitOfWork.GetRepository<MyTest>()
            .Add(new MyTest { Id = 201, Name = "test for delete" });
        unitOfWork.GetRepository<MyTest>()
            .Add(new MyTest { Id = 203, Name = "test for delete" });
        try 
        {
            unitOfWork.SaveChanges();
        }
        catch{ }
        #endregion
    }

    [Fact]
    public async Task RetrieveListingTest()
    {
        //arrange
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        //act
        var data = unitOfWork.GetRepository<MyTest>()
            .GetAll(r => r.Id > 0);
        var dataAsync = await unitOfWork.GetRepository<MyTest>()
            .GetAllAsync(r => r.Id > 0, cancellationToken: cts.Token);
        var query = unitOfWork.ReaderRepository<MyTest>()
            .Query(r => r.Id > 0);
        var result = await query.ToListAsync(cts.Token);

        //assert
        Assert.IsType<List<MyTest>>(data);
        Assert.IsType<List<MyTest>>(dataAsync);
        Assert.IsType<List<MyTest>>(result);
        Assert.Equal(typeof(List<MyTest>), result.GetType());
    }

    [Fact]
    public async Task RetrieveSingleByIdTest()
    {
        //arrange
        int id = 1;
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        //act
        var data = unitOfWork.GetRepository<MyTest>()
            .Get(id);
        var result = await unitOfWork.GetRepository<MyTest>()
            .GetAsync(id, cts.Token);

        //assert
        Assert.NotNull(result);
        Assert.Equal(typeof(MyTest), result.GetType());
        Assert.Equal(data, result);
    }

    [Fact]
    public async Task RetrieveSingleByConditionTest()
    {
        //arrange
        int id = 1;
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        //act
        var data = unitOfWork.GetRepository<MyTest>()
            .Get(r => r.Id == id);
        var result = await unitOfWork.GetRepository<MyTest>()
            .GetAsync(r => r.Id == id, cts.Token);

        //assert
        Assert.NotNull(result);
        Assert.Equal(typeof(MyTest), result.GetType());
        Assert.Equal(data, result);
    }

    [Fact]
    public async Task CreateEntryTest()
    {
        //arrange
        var payload = new MyTest { Id = 1111, Name = "Test AddAsync" };
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        using(var scope = serviceProvider.CreateScope())
        {
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<WriterContext, ReaderContext>>();

            //act
            var result = await unitOfWork.GetRepository<MyTest>()
                .AddAsync(payload, cts.Token);
            await unitOfWork.SaveChangesAsync(cts.Token);

            //assert
            Assert.NotNull(result);
            Assert.Equal(payload, result);
        }
    }

    [Fact]
    public void CreateEntryTest2()
    {
        //arrange
        var payload = new MyTest { Id = 1100, Name = "Test Add" };

        using(var scope = serviceProvider.CreateScope())
        {
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<WriterContext, ReaderContext>>();

            //act
            var result = unitOfWork.GetRepository<MyTest>().Add(payload);
            unitOfWork.SaveChanges();

            //assert
            Assert.NotNull(result);
            Assert.Equal(payload, result);
        }
    }

    [Fact]
    public async Task MaxValueTest()
    {
        //arrange
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        //act
        var value = unitOfWork.GetRepository<MyTest>()
            .Max(r => r.Id > 0, f => f.Id);
        var result = await unitOfWork.GetRepository<MyTest>()
            .MaxAsync(r => r.Id > 0, f => f.Id, cts.Token);

        //assert
        Assert.Equal(value, result);
        Assert.IsType<int>(result);
    }

    [Fact]
    public async Task UpdateSingleEntryTest()
    {
        //arrange
        int expected = 1;
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        using (var scope = serviceProvider.CreateScope())
        {
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<WriterContext, ReaderContext>>();

            //act
            var data = await unitOfWork.GetRepository<MyTest>().GetAsync(1);
            data!.Name = "data updated";
            unitOfWork.GetRepository<MyTest>()
                .Update(data);
            var result = await unitOfWork.SaveChangesAsync(cts.Token);

            //assert
            Assert.Equal(expected, result);
        }
    }

    [Fact]
    public async Task UpdateMultiEntryTest()
    {
        //arrange
        int expected = 0;
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        using(var scope = serviceProvider.CreateScope())
        {
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<WriterContext, ReaderContext>>();

            //act
            var dataList = await unitOfWork.GetRepository<MyTest>()
                .Query(r => r.Id <= 3)
                .ToListAsync(cts.Token);
            expected = dataList.Count;
            dataList.ForEach(e => e.Name += " updated");
            unitOfWork.GetRepository<MyTest>().Update(dataList);
            var result = await unitOfWork.SaveChangesAsync(cts.Token);

            //assert
            Assert.Equal(expected, result);
        }
    }

    [Fact]
    public async Task DeleteSingleEntryTest()
    {
        //arrange
        int expected = 1;
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        using (var scope = serviceProvider.CreateScope())
        {
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<WriterContext, ReaderContext>>();

            //act
            var data = await unitOfWork.GetRepository<MyTest>().GetAsync(200);
            unitOfWork.GetRepository<MyTest>()
                .Delete(data!);
            var result = await unitOfWork.SaveChangesAsync(cts.Token);

            //assert
            Assert.Equal(expected, result);
        }
    }

    [Fact]
    public async Task DeleteMultiEntryTest()
    {
        //arrange
        int expected = 0;
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        using (var scope = serviceProvider.CreateScope())
        {
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<WriterContext, ReaderContext>>();

            //act
            var dataList = await unitOfWork.GetRepository<MyTest>()
                .Query(r => r.Id == 201 || r.Id == 202)
                .ToListAsync(cts.Token);
            expected = dataList.Count;
            unitOfWork.GetRepository<MyTest>().Delete(dataList);
            var result = await unitOfWork.SaveChangesAsync(cts.Token);

            //assert
            Assert.Equal(expected, result);
        }
    }
}
