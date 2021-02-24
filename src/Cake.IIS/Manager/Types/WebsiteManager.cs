#region Using Statements

using Microsoft.Web.Administration;

using Cake.Core;
using Cake.Core.Diagnostics;
#endregion



namespace Cake.IIS
{
    /// <summary>
    /// Class for managing websites
    /// </summary>
    public class WebsiteManager : BaseSiteManager
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteManager" /> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="log">The log.</param>
        public WebsiteManager(ICakeEnvironment environment, ICakeLog log)
            : base(environment, log)
        {

        }
        #endregion





        #region Methods
        /// <summary>
        /// Creates a new instance of the <see cref="WebsiteManager" /> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="log">The log.</param>
        /// <param name="server">The <see cref="ServerManager" /> to connect to.</param>
        /// <returns>a new instance of the <see cref="WebFarmManager" />.</returns>
        public static WebsiteManager Using(ICakeEnvironment environment, ICakeLog log, ServerManager server)
        {
            WebsiteManager manager = new WebsiteManager(environment, log);

            manager.SetServer(server);

            return manager;
        }



        /// <summary>
        /// Creates a website
        /// </summary>
        /// <param name="settings">The settings of the website to add</param>
        public void Create(WebsiteSettings settings)
        {
            bool exists;
            Site site = base.CreateSite(settings, out exists);

            if (!exists)
            {
                _Server.CommitChanges();

                // Settings which needs to be modified after the site is created.
                var isModified = false;
                if (settings.EnableDirectoryBrowsing)
                {
                    var siteConfig = site.GetWebConfiguration();
                    siteConfig.EnableDirectoryBrowsing();
                    isModified = true;
                }
                if (isModified)
                {
                    _Server.CommitChanges();
                }

                _Log.Information("Web Site '{0}' created.", settings.Name);
            }
        }
        #endregion
    }
}