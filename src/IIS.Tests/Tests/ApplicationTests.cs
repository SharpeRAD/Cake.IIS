using Cake.IIS.Manager.Types;
using Cake.IIS.Tests.Utils;
using Xunit;

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

        [Fact]
        public void Should_Delete_App()
        {
            CakeHelper.CreateWebsite(CakeHelper.GetWebsiteSettings());
            var appSettings = CakeHelper.GetVirtualAppSettings();
            CakeHelper.CreateVirtualApplication(appSettings);

            CakeHelper.DeleteApplication(appSettings.ParentWebSite, appSettings.Name);

            Assert.Null(CakeHelper.GetVirtualApplication(appSettings.ParentWebSite, appSettings.Name));
        }
        
    }
}