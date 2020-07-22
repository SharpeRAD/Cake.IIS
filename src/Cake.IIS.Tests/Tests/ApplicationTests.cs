#region Using Statements
using System;

using Xunit;
using Shouldly;
#endregion



namespace Cake.IIS.Tests
{
    public class ApplicationTests
    {
        [Fact]
        public void Should_Create_Application()
        {
            // Arrange
            var websiteSettings = CakeHelper.GetWebsiteSettings("Superman");
            CakeHelper.DeleteWebsite(websiteSettings.Name);

            // Act
            CakeHelper.CreateWebsite(websiteSettings);
            var appSettings = CakeHelper.GetApplicationSettings(websiteSettings.Name);

            WebsiteManager manager = CakeHelper.CreateWebsiteManager();
            var added = manager.AddApplication(appSettings);

            // Assert
            added.ShouldBeTrue();
            CakeHelper.GetApplication(websiteSettings.Name, appSettings.ApplicationPath).ShouldNotBeNull();

            CakeHelper.DeleteWebsite(websiteSettings.Name);
        }

        [Fact]
        public void Should_Create_Application_With_Predefined_EnabledProtocols()
        {
            // Arrange
            var websiteSettings = CakeHelper.GetWebsiteSettings("Batman");
            CakeHelper.CreateWebsite(websiteSettings);

            // Act
            var appSettings = CakeHelper.GetApplicationSettings(websiteSettings.Name);
            appSettings.AlternateEnabledProtocols = "http,net.pipe";

            WebsiteManager manager = CakeHelper.CreateWebsiteManager();
            var added = manager.AddApplication(appSettings);

            // Assert
            added.ShouldBeTrue();
            var application = CakeHelper.GetApplication(websiteSettings.Name, appSettings.ApplicationPath);

            application.ShouldNotBeNull();
            application.EnabledProtocols.ShouldContain(BindingProtocol.Http.ToString());
            application.EnabledProtocols.ShouldContain(BindingProtocol.NetPipe.ToString());

            CakeHelper.DeleteWebsite(websiteSettings.Name);
        }

        [Fact]
        public void Should_Create_Application_With_DirectoryBrowsing()
        {
            // Arrange
            var websiteSettings = CakeHelper.GetWebsiteSettings("Hulk");
            CakeHelper.DeleteWebsite(websiteSettings.Name);

            // Act
            var appSettings = CakeHelper.GetApplicationSettings(websiteSettings.Name);
            CakeHelper.CreateWebConfig(appSettings);

            var manager = CakeHelper.CreateWebsiteManager();
            manager.Create(websiteSettings);
            manager.AddApplication(appSettings);
            manager.SetWebConfiguration(websiteSettings.Name, appSettings.ApplicationPath, config => config.EnableDirectoryBrowsing());

            // Assert
            var value = CakeHelper.GetWebConfigurationValue(websiteSettings.Name, appSettings.ApplicationPath, "system.webServer/directoryBrowse", "enabled");
            ((bool)value).ShouldBeTrue();

            CakeHelper.DeleteWebsite(websiteSettings.Name);
        }

        [Fact]
        public void Should_Create_Application_Without_DirectoryBrowsing()
        {
            // Arrange
            var websiteSettings = CakeHelper.GetWebsiteSettings("Smash");
            CakeHelper.DeleteWebsite(websiteSettings.Name);

            // Act
            var appSettings = CakeHelper.GetApplicationSettings(websiteSettings.Name);
            CakeHelper.CreateWebConfig(appSettings);

            var manager = CakeHelper.CreateWebsiteManager();
            manager.Create(websiteSettings);
            manager.AddApplication(appSettings);
            manager.SetWebConfiguration(websiteSettings.Name, appSettings.ApplicationPath, config => config.DisableDirectoryBrowsing());

            // Assert
            var value = CakeHelper.GetWebConfigurationValue(websiteSettings.Name, appSettings.ApplicationPath, "system.webServer/directoryBrowse", "enabled");
            ((bool)value).ShouldBeFalse();

            CakeHelper.DeleteWebsite(websiteSettings.Name);
        }

        [Fact]
        public void Should_Create_Application_With_DirectoryBrowsing_In_Settings()
        {
            // Arrange
            var websiteSettings = CakeHelper.GetWebsiteSettings("Bruce");
            CakeHelper.DeleteWebsite(websiteSettings.Name);

            // Act
            var appSettings = CakeHelper.GetApplicationSettings(websiteSettings.Name);
            appSettings.EnableDirectoryBrowsing = true;
            CakeHelper.CreateWebConfig(appSettings);

            var manager = CakeHelper.CreateWebsiteManager();
            manager.Create(websiteSettings);
            manager.AddApplication(appSettings);

            // Assert
            var value = CakeHelper.GetWebConfigurationValue(websiteSettings.Name, appSettings.ApplicationPath, "system.webServer/directoryBrowse", "enabled");
            ((bool)value).ShouldBeTrue();

            CakeHelper.DeleteWebsite(websiteSettings.Name);
        }

        [Fact(Skip = "unknown bug")]
        public void Should_Create_Application_Without_DirectoryBrowsing_In_Settings()
        {
            //Setup
            var websiteSettings = CakeHelper.GetWebsiteSettings("Banner");
            CakeHelper.DeleteWebsite(websiteSettings.Name);

            // Arrange
            var appSettings = CakeHelper.GetApplicationSettings(websiteSettings.Name);
            appSettings.EnableDirectoryBrowsing = false;

            // Make sure the web.config exists
            CakeHelper.CreateWebConfig(appSettings);

            // Act
            var manager = CakeHelper.CreateWebsiteManager();
            manager.Create(websiteSettings);
            manager.AddApplication(appSettings);

            // Assert
            var value = CakeHelper.GetWebConfigurationValue(websiteSettings.Name, appSettings.ApplicationPath, "system.webServer/directoryBrowse", "enabled");
            ((bool)value).ShouldBeFalse();

            //Teardown
            CakeHelper.DeleteWebsite(websiteSettings.Name);
        }
    }
}