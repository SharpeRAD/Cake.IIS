using Xunit;

namespace Cake.IIS.Tests
{
    public class BindingTests
    {
        [Fact]
        public void Should_Add_Ftp_Binding()
        {
            // Arrange
            var settings = CreateWebSite();
            var bindingSettings = IISBindings.Ftp;

            // Act
            Act(settings.Name, bindingSettings);

            // Assert
            var website = CakeHelper.GetWebsite(settings.Name);
            Assert.NotNull(website);
            Assert.Equal(2, website.Bindings.Count);
            Assert.Contains(website.Bindings, b => b.Protocol == BindingProtocol.Ftp.ToString() &&
                                                   b.BindingInformation == bindingSettings.BindingInformation);
        }

        [Fact]
        public void Should_Add_NetTcp_Binding()
        {
            // Arrange
            var settings = CreateWebSite();
            BindingSettings bindingSettings = IISBindings.NetTcp;

            // Act
            Act(settings.Name, bindingSettings);

            // Assert
            var website = CakeHelper.GetWebsite(settings.Name);
            Assert.NotNull(website);
            Assert.Equal(2, website.Bindings.Count);
            Assert.Contains(website.Bindings, b => b.Protocol == BindingProtocol.NetTcp.ToString() &&
                                                   b.BindingInformation == bindingSettings.BindingInformation);
        }

        [Fact]
        public void Should_Add_NetPipe_Binding()
        {
            // Arrange
            var settings = CreateWebSite();
            BindingSettings bindingSettings = IISBindings.NetPipe;

            // Act
            Act(settings.Name, bindingSettings);

            // Assert
            var website = CakeHelper.GetWebsite(settings.Name);
            Assert.NotNull(website);
            Assert.Equal(2, website.Bindings.Count);
            Assert.Contains(website.Bindings, b => b.Protocol == BindingProtocol.NetPipe.ToString() &&
                                                   b.BindingInformation == bindingSettings.BindingInformation);
        }

        [Fact]
        public void Should_Add_NetMsmq_Binding()
        {
            // Arrange
            var settings = CreateWebSite();
            BindingSettings bindingSettings = IISBindings.NetMsmq;

            // Act
            Act(settings.Name, bindingSettings);

            // Assert
            var website = CakeHelper.GetWebsite(settings.Name);
            Assert.NotNull(website);
            Assert.Equal(2, website.Bindings.Count);
            Assert.Contains(website.Bindings, b => b.Protocol == BindingProtocol.NetMsmq.ToString() &&
                                                   b.BindingInformation == bindingSettings.BindingInformation);
        }

        [Fact]
        public void Should_Add_MsmqFormatName_Binding()
        {
            // Arrange
            var settings = CreateWebSite();
            BindingSettings bindingSettings = IISBindings.MsmqFormatName;

            // Act
            Act(settings.Name, bindingSettings);

            // Assert
            var website = CakeHelper.GetWebsite(settings.Name);
            Assert.NotNull(website);
            Assert.Equal(2, website.Bindings.Count);
            Assert.Contains(website.Bindings, b => b.Protocol == BindingProtocol.MsmqFormatName.ToString() &&
                                                   b.BindingInformation == bindingSettings.BindingInformation);
        }

        private WebsiteSettings CreateWebSite()
        {
            // Arrange
            var settings = CakeHelper.GetWebsiteSettings();
            CakeHelper.DeleteWebsite(settings.Name);
            WebsiteManager manager = CakeHelper.CreateWebsiteManager();
            manager.Create(settings);

            return settings;
        }

        private void Act(string siteName, BindingSettings bindingSettings)
        {
            WebsiteManager manager = CakeHelper.CreateWebsiteManager();
            manager.AddBinding(siteName, bindingSettings);
        }
    }
}