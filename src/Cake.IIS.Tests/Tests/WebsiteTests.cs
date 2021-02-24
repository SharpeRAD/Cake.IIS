#region Using Statements

using Microsoft.Web.Administration;

using Xunit;
using Shouldly;
#endregion



namespace Cake.IIS.Tests
{
    public class WebsiteTests
    {
        [Fact]
        public void Should_Create_Website()
        {
            // Arrange
            var settings = CakeHelper.GetWebsiteSettings("Deadpool");
            CakeHelper.DeleteWebsite(settings.Name);

            // Act
            WebsiteManager manager = CakeHelper.CreateWebsiteManager();
            manager.Create(settings);

            // Assert
            CakeHelper.GetWebsite(settings.Name).ShouldNotBeNull();
        }

        [Fact]
        public void Should_Create_Website_With_DirectoryBrowsing()
        {
            // Arrange
            var websiteSettings = CakeHelper.GetWebsiteSettings("Tony");

            // Make sure the web.config exists
            CakeHelper.CreateWebConfig(websiteSettings);

            // Act
            var manager = CakeHelper.CreateWebsiteManager();
            manager.Create(websiteSettings);
            manager.SetWebConfiguration(websiteSettings.Name, null, config => config.EnableDirectoryBrowsing());

            // Assert
            var value = CakeHelper.GetWebConfigurationValue(websiteSettings.Name, null, "system.webServer/directoryBrowse", "enabled");
            ((bool)value).ShouldBeTrue();

            CakeHelper.DeleteWebsite(websiteSettings.Name);
        }

        [Fact]
        public void Should_Create_Website_Without_DirectoryBrowsing()
        {
            // Arrange
            var websiteSettings = CakeHelper.GetWebsiteSettings("Stark");

            // Make sure the web.config exists
            CakeHelper.CreateWebConfig(websiteSettings);

            // Act
            var manager = CakeHelper.CreateWebsiteManager();
            manager.Create(websiteSettings);
            manager.SetWebConfiguration(websiteSettings.Name, null, config => config.DisableDirectoryBrowsing());

            // Assert
            var value = CakeHelper.GetWebConfigurationValue(websiteSettings.Name, null, "system.webServer/directoryBrowse", "enabled");
            ((bool)value).ShouldBeFalse();

            CakeHelper.DeleteWebsite(websiteSettings.Name);
        }

        [Fact]
        public void Should_Create_Website_With_DirectoryBrowsing_In_Settings()
        {
            // Arrange
            var websiteSettings = CakeHelper.GetWebsiteSettings("Iron");
            websiteSettings.EnableDirectoryBrowsing = true;

            // Make sure the web.config exists
            CakeHelper.CreateWebConfig(websiteSettings);

            // Act
            var manager = CakeHelper.CreateWebsiteManager();
            manager.Create(websiteSettings);
            manager.SetWebConfiguration(websiteSettings.Name, null, config => config.EnableDirectoryBrowsing());

            // Assert
            var value = CakeHelper.GetWebConfigurationValue(websiteSettings.Name, null, "system.webServer/directoryBrowse", "enabled");
            ((bool)value).ShouldBeTrue();

            CakeHelper.DeleteWebsite(websiteSettings.Name);
        }

        [Fact]
        public void Should_Create_Website_Without_DirectoryBrowsing_In_Settings()
        {
            // Arrange
            var websiteSettings = CakeHelper.GetWebsiteSettings("Man");
            websiteSettings.EnableDirectoryBrowsing = false;

            // Make sure the web.config exists
            CakeHelper.CreateWebConfig(websiteSettings);

            // Act
            var manager = CakeHelper.CreateWebsiteManager();
            manager.Create(websiteSettings);
            manager.SetWebConfiguration(websiteSettings.Name, null, config => config.DisableDirectoryBrowsing());

            // Assert
            var value = CakeHelper.GetWebConfigurationValue(websiteSettings.Name, null, "system.webServer/directoryBrowse", "enabled");
            ((bool)value).ShouldBeFalse();

            CakeHelper.DeleteWebsite(websiteSettings.Name);
        }

        [Fact]
        public void Should_Create_Website_With_Fluently_Defined_Binding()
        {
            // Arrange
            var settings = CakeHelper.GetWebsiteSettings("Thor");
            const string expectedHostName = "Thor.web";
            const string expectedIpAddress = "*";
            const int expectedPort = 981;

            settings.Binding = IISBindings.Http
                .SetHostName(expectedHostName)
                .SetIpAddress(expectedIpAddress)
                .SetPort(expectedPort);

            CakeHelper.DeleteWebsite(settings.Name);

            // Act
            WebsiteManager manager = CakeHelper.CreateWebsiteManager();
            manager.Create(settings);

            // Assert
            var binding = settings.Binding;
            var website = CakeHelper.GetWebsite(settings.Name);

            website.ShouldNotBeNull();
            website.Bindings.Count.ShouldBe(1);
            website.Bindings.ShouldContain(b => b.Protocol == BindingProtocol.Http.ToString() &&
                                                   b.BindingInformation == binding.BindingInformation &&
                                                   b.BindingInformation.Contains(expectedPort.ToString()) &&
                                                   b.BindingInformation.Contains(expectedHostName) &&
                                                   b.BindingInformation.Contains(expectedIpAddress));
        }

        [Fact]
        public void Should_Create_Website_With_Directly_Defined_Binding()
        {
            // Arrange
            var settings = CakeHelper.GetWebsiteSettings("CaptainAmerica");
            const string expectedHostName = "CaptainAmerica.web";
            const string expectedIpAddress = "*";
            const int expectedPort = 981;

            var binding = new BindingSettings(BindingProtocol.Http)
            {
                HostName = expectedHostName,
                IpAddress = expectedIpAddress,
                Port = expectedPort,
            };
            settings.Binding = binding;

            CakeHelper.DeleteWebsite(settings.Name);

            // Act
            WebsiteManager manager = CakeHelper.CreateWebsiteManager();
            manager.Create(settings);

            // Assert
            var website = CakeHelper.GetWebsite(settings.Name);

            website.ShouldNotBeNull();
            website.Bindings.Count.ShouldBe(1);
            website.Bindings.ShouldContain(b => b.Protocol == BindingProtocol.Http.ToString() &&
                                                   b.BindingInformation == binding.BindingInformation &&
                                                   b.BindingInformation.Contains(expectedPort.ToString()) &&
                                                   b.BindingInformation.Contains(expectedHostName) &&
                                                   b.BindingInformation.Contains(expectedIpAddress));
        }

        [Fact]
        public void Should_Create_Website_With_Predefined_EnabledProtocols()
        {
            // Arrange
            var settings = CakeHelper.GetWebsiteSettings("Vision");
            settings.AlternateEnabledProtocols = "http,net.msmq,net.tcp";
            CakeHelper.DeleteWebsite(settings.Name);

            // Act
            WebsiteManager manager = CakeHelper.CreateWebsiteManager();
            manager.Create(settings);

            // Assert
            var website = CakeHelper.GetWebsite(settings.Name);

            website.ShouldNotBeNull();
            website.ApplicationDefaults.EnabledProtocols.Contains(BindingProtocol.Http.ToString());
            website.ApplicationDefaults.EnabledProtocols.Contains(BindingProtocol.NetMsmq.ToString());
            website.ApplicationDefaults.EnabledProtocols.Contains(BindingProtocol.NetTcp.ToString());
        }

        [Fact]
        public void Should_Delete_Website()
        {
            // Arrange
            var settings = CakeHelper.GetWebsiteSettings("Wasp");
            CakeHelper.CreateWebsite(settings);

            // Act
            CakeHelper.CreateWebsiteManager().Delete(settings.Name);

            // Assert
            CakeHelper.GetWebsite(settings.Name).ShouldBeNull();
        }

        [Fact]
        public void Should_Start_Website()
        {
            // Arrange
            var settings = CakeHelper.GetWebsiteSettings("Sunspot");

            CakeHelper.CreateWebsite(settings);
            CakeHelper.StopWebsite(settings.Name);

            // Act
            CakeHelper.CreateWebsiteManager().Start(settings.Name);

            // Assert
            Site site = CakeHelper.GetWebsite(settings.Name);

            site.ShouldNotBeNull();
            site.State.ShouldBe(ObjectState.Started);
        }

        [Fact]
        public void Should_Stop_Website()
        {
            // Arrange
            var settings = CakeHelper.GetWebsiteSettings("HumanTourch");

            CakeHelper.CreateWebsite(settings);
            CakeHelper.StartWebsite(settings.Name);

            // Act
            CakeHelper.CreateWebsiteManager().Stop(settings.Name);

            // Assert
            Site site = CakeHelper.GetWebsite(settings.Name);

            site.ShouldNotBeNull();
            site.State.ShouldBe(ObjectState.Stopped);
        }
    }
}