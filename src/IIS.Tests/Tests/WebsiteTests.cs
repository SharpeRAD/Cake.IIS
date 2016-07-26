#region Using Statements

using System.IO;
using Cake.Core.IO;
using Cake.IIS.Tests.Utils;
    using Microsoft.Web.Administration;
    using Xunit;
#endregion



namespace Cake.IIS.Tests
{
    public class WebsiteTests
    {


        [Fact]
        public void Should_Create_Website()
        {
            // Arrange
            var settings = CakeHelper.GetWebsiteSettings();
            CakeHelper.DeleteWebsite(settings.Name);

            // Act
            WebsiteManager manager = CakeHelper.CreateWebsiteManager();
            manager.Create(settings);

            // Assert
            Assert.NotNull(CakeHelper.GetWebsite(settings.Name));
        }
        [Fact]
        public void Should_Create_App()
        {
            // Arrange
            var settings = CakeHelper.GetWebsiteSettings();
            CakeHelper.DeleteWebsite(settings.Name);
            WebsiteManager manager = CakeHelper.CreateWebsiteManager();
            manager.Create(settings);
            Directory.CreateDirectory("Test");

            // Act
            CakeHelper.CreateApplication(CakeHelper.GetAppSettings());
            // Assert
            Assert.NotNull(CakeHelper.GetWebsite(settings.Name));
        }

        [Fact]
        public void Should_Delete_Website()
        {
            // Arrange
            var settings = CakeHelper.GetWebsiteSettings();
            CakeHelper.CreateWebsite(settings);

            // Act
            CakeHelper.CreateWebsiteManager().Delete(settings.Name);

            // Assert
            Assert.Null(CakeHelper.GetWebsite(settings.Name));
        }

        [Fact]
        public void Should_Start_Website()
        {
            // Arrange
            var settings = CakeHelper.GetWebsiteSettings();

            CakeHelper.CreateWebsite(settings);
            CakeHelper.StopWebsite(settings.Name);

            // Act
            CakeHelper.CreateWebsiteManager().Start(settings.Name);

            // Assert
            Site site = CakeHelper.GetWebsite(settings.Name);

            Assert.NotNull(site);
            Assert.True(site.State == ObjectState.Started);
        }

        [Fact]
        public void Should_Stop_Website()
        {
            // Arrange
            var settings = CakeHelper.GetWebsiteSettings();

            CakeHelper.CreateWebsite(settings);
            CakeHelper.StartWebsite(settings.Name);

            // Act
            CakeHelper.CreateWebsiteManager().Stop(settings.Name);

            // Assert
            Site site = CakeHelper.GetWebsite(settings.Name);

            Assert.NotNull(site);
            Assert.True(site.State == ObjectState.Stopped);
        }
    }
}