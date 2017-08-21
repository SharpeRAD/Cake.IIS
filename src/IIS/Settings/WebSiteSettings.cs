namespace Cake.IIS
{
    public class WebsiteSettings : SiteSettings
    {
        #region Constructor (1)
        public WebsiteSettings()
            : base()
        {
            Binding = IISBindings.Http;
        }
        #endregion

        /// <summary>
        /// Flag to get or set of directory browsing should be enabled
        /// </summary>
        public bool EnableDirectoryBrowsing
        {
            get;
            set;
        }
    }
}
