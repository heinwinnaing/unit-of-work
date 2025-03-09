using Moq;
namespace UnitOfWork.Test
{
    public class UnitOfWorkTest
    {
        [Fact]
        public void TestUOWSuccess()
        {
            //arrange
            var expected = new { id = 1, name = "test" };
            var mockUow = new Mock<IUnitOfWork>();
            mockUow.Setup(s => s.GetRepository<object>().Get(r => 1 ==1))
                .Returns(new { id = 1, name = "test" });

            //action
            var result = mockUow.Object.GetRepository<object>().Get(r => 1 == 1);

            //assert
            Assert.NotNull(result);
            Assert.Equal(expected, result);
        }
    }
}
