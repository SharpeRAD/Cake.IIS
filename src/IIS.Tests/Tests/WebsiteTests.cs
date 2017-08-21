#region Using Statements
using System;

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
            var settings = CakeHelper.GetWebsiteSettings("Deadpool");
            CakeHelper.DeleteWebsite(settings.Name);

            // Act
            WebsiteManager manager = CakeHelper.CreateWebsiteManager();
            manager.Create(settings);

            // Assert
            Assert.NotNull(CakeHelper.GetWebsite(settings.Name));

            // Cleanup
            CakeHelper.DeleteWebsite(settings.Name);
        }

        [Fact]
        public void Should_Create_Website_With_DirectoryBrowsing()
        {
            // Arrange
            var settings = CakeHelper.GetWebsiteSettings("Tony");
            settings.EnableDirectoryBrowsing = true;
            CakeHelper.DeleteWebsite(settings.Name);
            // Make sure the web.config exists
            CakeHelper.CreateWebConfig(settings);

            // Act
            WebsiteManager manager = CakeHelper.CreateWebsiteManager();
            manager.Create(settings);

            // Assert
            Assert.NotNull(CakeHelper.GetWebsite(settings.Name));
            var config = manager.ServerManager.GetWebConfiguration(settings.Name);
            var directoryBrowseSection = config.GetSection("system.webServer/directoryBrowse");
            Assert.True((bool)directoryBrowseSection["enabled"]);

            // Cleanup
            CakeHelper.DeleteWebsite(settings.Name);
        }

        [Fact]
        public void Should_Create_Website_Without_DirectoryBrowsing()
        {
            // Arrange
            var settings = CakeHelper.GetWebsiteSettings("Stark");
            settings.EnableDirectoryBrowsing = false;
            CakeHelper.DeleteWebsite(settings.Name);
            // Make sure the web.config exists
            CakeHelper.CreateWebConfig(settings);

            // Act
            WebsiteManager manager = CakeHelper.CreateWebsiteManager();
            manager.Create(settings);

            // Assert
            Assert.NotNull(CakeHelper.GetWebsite(settings.Name));
            var config = manager.ServerManager.GetWebConfiguration(settings.Name);
            var directoryBrowseSection = config.GetSection("system.webServer/directoryBrowse");
            Assert.False((bool)directoryBrowseSection["enabled"]);

            // Cleanup
            CakeHelper.DeleteWebsite(settings.Name);
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
            Assert.NotNull(website);
            Assert.Equal(1, website.Bindings.Count);
            Assert.Contains(website.Bindings, b => b.Protocol == BindingProtocol.Http.ToString() &&
                                                   b.BindingInformation == binding.BindingInformation &&
                                                   b.BindingInformation.Contains(expectedPort.ToString()) &&
                                                   b.BindingInformation.Contains(expectedHostName) &&
                                                   b.BindingInformation.Contains(expectedIpAddress));

            // Cleanup
            CakeHelper.DeleteWebsite(settings.Name);
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
            Assert.NotNull(website);
            Assert.Equal(1, website.Bindings.Count);
            Assert.Contains(website.Bindings, b => b.Protocol == BindingProtocol.Http.ToString() &&
                                                   b.BindingInformation == binding.BindingInformation &&
                                                   b.BindingInformation.Contains(expectedPort.ToString()) &&
                                                   b.BindingInformation.Contains(expectedHostName) &&
                                                   b.BindingInformation.Contains(expectedIpAddress));

            // Cleanup
            CakeHelper.DeleteWebsite(settings.Name);
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
            Assert.NotNull(website);
            Assert.Contains(BindingProtocol.Http.ToString(),
                website.ApplicationDefaults.EnabledProtocols,
                StringComparison.OrdinalIgnoreCase);
            Assert.Contains(BindingProtocol.NetMsmq.ToString(),
                website.ApplicationDefaults.EnabledProtocols,
                StringComparison.OrdinalIgnoreCase);
            Assert.Contains(BindingProtocol.NetTcp.ToString(),
                website.ApplicationDefaults.EnabledProtocols,
                StringComparison.OrdinalIgnoreCase);

            // Cleanup
            CakeHelper.DeleteWebsite(settings.Name);
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
            Assert.Null(CakeHelper.GetWebsite(settings.Name));

            // Cleanup
            CakeHelper.DeleteWebsite(settings.Name);
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

            Assert.NotNull(site);
            Assert.True(site.State == ObjectState.Started);

            // Cleanup
            CakeHelper.DeleteWebsite(settings.Name);
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

            Assert.NotNull(site);
            Assert.True(site.State == ObjectState.Stopped);

            // Cleanup
            CakeHelper.DeleteWebsite(settings.Name);
        }
    }
}