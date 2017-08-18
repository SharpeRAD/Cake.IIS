using Microsoft.Web.Administration;
using Xunit;

namespace Cake.IIS.Tests
{
    public class ApplicationPoolTests
    {
        [Fact]
        public void Should_Create_Delete_AppPool()
        {
            // Arrange
            var settings = CakeHelper.GetAppPoolSettings("Bart");

            // Create
            CakeHelper.CreateApplicationPoolManager().Create(settings);
            Assert.NotNull(CakeHelper.GetPool(settings.Name));

            // Delete
            CakeHelper.DeletePool(settings.Name);
            Assert.Null(CakeHelper.GetPool(settings.Name));
        }

        [Fact]
        public void Should_Start_AppPool()
        {
            // Arrange
            var settings = CakeHelper.GetAppPoolSettings("Homer");

            CakeHelper.CreatePool(settings);
            CakeHelper.StopPool(settings.Name);

            // Act
            CakeHelper.CreateApplicationPoolManager().Start(settings.Name);

            // Assert
            ApplicationPool pool = CakeHelper.GetPool(settings.Name);

            Assert.NotNull(pool);
            Assert.True(pool.State == ObjectState.Started);

            CakeHelper.DeletePool(settings.Name);
        }

        [Fact]
        public void Should_Stop_AppPool()
        {
            // Arrange
            var settings = CakeHelper.GetAppPoolSettings("Marg");

            CakeHelper.CreatePool(settings);
            CakeHelper.StartPool(settings.Name);

            // Act
            CakeHelper.CreateApplicationPoolManager().Stop(settings.Name);

            // Assert
            ApplicationPool pool = CakeHelper.GetPool(settings.Name);

            Assert.NotNull(pool);
            Assert.True(pool.State == ObjectState.Stopped);

            CakeHelper.DeletePool(settings.Name);
        }
    }
}