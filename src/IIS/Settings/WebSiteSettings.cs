namespace Cake.IIS
{
    public class WebsiteSettings : SiteSettings
    {
        public WebsiteSettings()
        {
            Binding = IISBindings.Http;
        }
    }
}
