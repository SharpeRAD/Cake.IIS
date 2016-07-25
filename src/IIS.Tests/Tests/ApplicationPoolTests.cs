#region Using Statements

using System.IO;
using Cake.IIS.Tests.Utils;
    using Microsoft.Web.Administration;
    using Xunit;
#endregion



namespace Cake.IIS.Tests
{
    public class ApplicationTests
    {
        [Fact]
        public void Should_Create_App()
        {
            // Arrange
            var siteSettings = CakeHelper.GetWebsiteSettings();
            var appSettings = CakeHelper.GetVirtualAppSettings();
            CakeHelper.CreateWebsite(siteSettings);
            

            // Act
            CakeHelper.CreateVirtualApplication(appSettings);

            Assert.NotNull(CakeHelper.GetVirtualApplication(appSettings.ParentWebSite, appSettings.Name));
        }
    }
    public class ApplicationPoolTests
    {
        [Fact]
        public void Should_Create_AppPool()
        {
            // Arrange
            var settings = CakeHelper.GetAppPoolSettings();
            CakeHelper.DeletePool(settings.Name);
            
            // Act 
            CakeHelper.CreateApplicationPoolManager().Create(settings);

            // Assert
            Assert.NotNull(CakeHelper.GetPool(settings.Name));
        }

        [Fact]
        public void Should_Delete_AppPool()
        {
            // Arrange
            var settings = CakeHelper.GetAppPoolSettings();
            CakeHelper.CreatePool(settings);

            // Act
            CakeHelper.CreateApplicationPoolManager().Delete(settings.Name);

            // Assert
            Assert.Null(CakeHelper.GetPool(settings.Name));
        }

        [Fact]
        public void Should_Start_AppPool()
        {
            // Arrange
            var settings = CakeHelper.GetAppPoolSettings();

            CakeHelper.CreatePool(settings);
            CakeHelper.StopPool(settings.Name);

            // Act
            CakeHelper.CreateApplicationPoolManager().Start(settings.Name);

            // Assert
            ApplicationPool pool = CakeHelper.GetPool(settings.Name);

            Assert.NotNull(pool);
            Assert.True(pool.State == ObjectState.Started);
        }

        [Fact]
        public void Should_Stop_AppPool()
        {
            // Arrange
            var settings = CakeHelper.GetAppPoolSettings();

            CakeHelper.CreatePool(settings);
            CakeHelper.StartPool(settings.Name);

            // Act
            CakeHelper.CreateApplicationPoolManager().Stop(settings.Name);

            // Assert
            ApplicationPool pool = CakeHelper.GetPool(settings.Name);

            Assert.NotNull(pool);
            Assert.True(pool.State == ObjectState.Stopped);
        }
    }
}