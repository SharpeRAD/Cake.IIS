using System;
using Xunit;

namespace Cake.IIS.Tests
{
    public class ApplicationTests
    {
        [Fact]
        public void Should_Create_Application()
        {
            // Arrange
            var websiteSettings = CakeHelper.GetWebsiteSettings("Superman");
            CakeHelper.CreateWebsite(websiteSettings);

            var appSettings = CakeHelper.GetApplicationSettings(websiteSettings.Name);

            // Act
            WebsiteManager manager = CakeHelper.CreateWebsiteManager();
            var added = manager.AddApplication(appSettings);

            // Assert
            Assert.True(added);
            Assert.NotNull(CakeHelper.GetApplication(websiteSettings.Name, appSettings.ApplicationPath));

            CakeHelper.DeleteWebsite(websiteSettings.Name);
        }

        [Fact]
        public void Should_Create_Application_With_Predefined_EnabledProtocols()
        {
            // Arrange
            var websiteSettings = CakeHelper.GetWebsiteSettings("Batman");
            CakeHelper.CreateWebsite(websiteSettings);

            var appSettings = CakeHelper.GetApplicationSettings(websiteSettings.Name);
            appSettings.AlternateEnabledProtocols = "http,net.pipe";

            // Act
            WebsiteManager manager = CakeHelper.CreateWebsiteManager();
            var added = manager.AddApplication(appSettings);

            // Assert
            Assert.True(added);
            var application = CakeHelper.GetApplication(websiteSettings.Name, appSettings.ApplicationPath);
            Assert.NotNull(application);
            Assert.Contains(BindingProtocol.Http.ToString(), 
                application.EnabledProtocols, 
                StringComparison.OrdinalIgnoreCase);
            Assert.Contains(BindingProtocol.NetPipe.ToString(), 
                application.EnabledProtocols, 
                StringComparison.OrdinalIgnoreCase);

            CakeHelper.DeleteWebsite(websiteSettings.Name);
        }
    }
}