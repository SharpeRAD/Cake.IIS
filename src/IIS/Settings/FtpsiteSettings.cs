namespace Cake.IIS
{
    public class FtpsiteSettings : SiteSettings
    {
        public FtpsiteSettings()
        {
            Binding = IISBindings.Ftp;
        }
    }
}
