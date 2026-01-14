using Microsoft.EntityFrameworkCore;
using Moq;
namespace UnitOfWork.Test;

public record TestClass
{
    public int Id { get; set; }
    public string? Name { get; set; }
}
public class TestDbWriter : DbContext { }
public class TestDbReader : DbContext { }
public class UnitOfWorkTest
{
    private readonly Mock<IUnitOfWork<TestDbWriter, TestDbReader>> mockUow;
    public UnitOfWorkTest()
    {
        mockUow = new Mock<IUnitOfWork<TestDbWriter, TestDbReader>>();
    }

    [Fact]
    public void TestUOWSuccess()
    {
        //arrange
        var expected = new TestClass { Id = 1, Name = "test" };
        mockUow.Setup(s => s.GetRepository<TestClass>().Get(r => 1 ==1))
            .Returns(new TestClass { Id = 1, Name = "test" });

        //action
        var result = mockUow.Object.GetRepository<TestClass>().Get(r => 1 == 1);

        //assert
        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void UowWriteContextTestSuccess()
    {
        //arrange
        var expected = new TestClass { Id = 1, Name = "Writer test" };
        var payload = new TestClass { Id = 1, Name = "Writer test" };
        mockUow.Setup(r => r.ReaderRepository<TestClass>().Add(payload))
            .Returns(payload);

        //act
        var result = mockUow.Object.ReaderRepository<TestClass>().Add(payload);

        //assert
        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void UowReaderContextTestSuccess()
    {
        //arrange
        var expected = new TestClass { Id = 1, Name = "Writer test" };
        mockUow.Setup(r => r.ReaderRepository<TestClass>().Get(1))
            .Returns(new TestClass { Id = 1, Name = "Writer test" });

        //act
        var result = mockUow.Object.ReaderRepository<TestClass>().Get(1);

        //assert
        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }
}
