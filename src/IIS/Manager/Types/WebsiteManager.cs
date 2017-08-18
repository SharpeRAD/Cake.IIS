using Microsoft.Web.Administration;
using Cake.Core;
using Cake.Core.Diagnostics;

namespace Cake.IIS
{
    /// <summary>
    /// Class for managing websites
    /// </summary>
    public class WebsiteManager : BaseSiteManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteManager" /> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="log">The log.</param>
        public WebsiteManager(ICakeEnvironment environment, ICakeLog log)
            : base(environment, log)
        {
        }

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
            Site site = CreateSite(settings, out exists);

            if (!exists)
            {
                Server.CommitChanges();
                Log.Information("Web Site '{0}' created.", settings.Name);
            }
        }
    }
}