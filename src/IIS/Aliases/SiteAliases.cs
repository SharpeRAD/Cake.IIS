using Cake.Core;
using Cake.Core.Annotations;
using Microsoft.Web.Administration;

namespace Cake.IIS
{
    /// <summary>
    /// Contains aliases for working with IIS sites.
    /// </summary>
    [CakeAliasCategory("IIS")]
    [CakeNamespaceImport("Microsoft.Web.Administration")]
    public static class SiteAliases
    {
        /// <summary>
        /// Checks if site exists on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The site name.</param>
        /// <returns><c>true</c> if exists</returns>
        [CakeMethodAlias]
        public static bool SiteExists(this ICakeContext context, string name)
        {
            return context.SiteExists("", name);
        }

        /// <summary>
        /// Checks if site exists on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="name">The site name.</param>
        /// <returns><c>true</c> if exists</returns>
        [CakeMethodAlias]
        public static bool SiteExists(this ICakeContext context, string server, string name)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                return WebsiteManager
                        .Using(context.Environment, context.Log, manager)
                        .Exists(name);
            }
        }

        /// <summary>
        /// Deletes site from local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The site name.</param>
        /// <returns><c>true</c> if deleted</returns>
        [CakeMethodAlias]
        public static bool DeleteSite(this ICakeContext context, string name)
        {
            return context.DeleteSite("", name);
        }

        /// <summary>
        /// Deletes site from remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="name">The site name.</param>
        /// <returns><c>true</c> if deleted</returns>
        [CakeMethodAlias]
        public static bool DeleteSite(this ICakeContext context, string server, string name)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                return WebsiteManager
                        .Using(context.Environment, context.Log, manager)
                        .Delete(name);
            }
        }

        /// <summary>
        /// Starts site on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The site name.</param>
        /// <returns><c>true</c> if started</returns>
        [CakeMethodAlias]
        public static bool StartSite(this ICakeContext context, string name)
        {
            return context.StartSite("", name);
        }

        /// <summary>
        /// Starts site on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="name">The site name.</param>
        /// <returns><c>true</c> if started</returns>
        [CakeMethodAlias]
        public static bool StartSite(this ICakeContext context, string server, string name)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                return WebsiteManager
                        .Using(context.Environment, context.Log, manager)
                        .Start(name);
            }
        }

        /// <summary>
        /// Stops site on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The site name.</param>
        /// <returns><c>true</c> if stopped</returns>
        [CakeMethodAlias]
        public static bool StopSite(this ICakeContext context, string name)
        {
            return context.StopSite("", name);
        }

        /// <summary>
        /// Stops site on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="name">The site name.</param>
        /// <returns><c>true</c> if stopped</returns>
        [CakeMethodAlias]
        public static bool StopSite(this ICakeContext context, string server, string name)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                return WebsiteManager
                        .Using(context.Environment, context.Log, manager)
                        .Stop(name);
            }
        }

        /// <summary>
        /// Restarts site on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The site name.</param>
        /// <returns><c>true</c> if restarted</returns>
        [CakeMethodAlias]
        public static bool RestartSite(this ICakeContext context, string name)
        {
            return context.RestartSite("", name);
        }

        /// <summary>
        /// Restarts site on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="name">The site name.</param>
        /// <returns><c>true</c> if restarted</returns>
        [CakeMethodAlias]
        public static bool RestartSite(this ICakeContext context, string server, string name)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                WebsiteManager webManager = WebsiteManager.Using(context.Environment, context.Log, manager);

                if (webManager.Stop(name))
                {
                    return webManager.Start(name);
                }
                return false;
            }
        }
    }
}
