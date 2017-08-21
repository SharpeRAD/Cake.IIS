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
        #region Constructor (1)
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





        #region Methods (2)
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
                // Commit the changes
                ServerManager.CommitChanges();
                _Log.Information("Web Site '{0}' created.", settings.Name);

                // Set directory browsing
                if (settings.EnableDirectoryBrowsing)
                {
                    var config = ServerManager.GetWebConfiguration(settings.Name);
                    var directoryBrowseSection = config.GetSection("system.webServer/directoryBrowse");
                    directoryBrowseSection["enabled"] = true;
                    directoryBrowseSection["showFlags"] = @"Date, Time, Size, Extension";
                    ServerManager.CommitChanges();
            }
        }
        }
        #endregion
    }
}