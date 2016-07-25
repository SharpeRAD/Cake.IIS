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

        
    }
}