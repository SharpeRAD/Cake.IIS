using Cake.Core;
using Cake.Core.Annotations;
using Microsoft.Web.Administration;

namespace Cake.IIS
{
    /// <summary>
    /// Contains aliases for working with IIS virtual applications.
    /// </summary>
    [CakeAliasCategory("IIS")]
    [CakeNamespaceImport("Microsoft.Web.Administration")]
    public static class VirtualDirectoryAliases
    {
        /// <summary>
        /// Adds site virtual directory to local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="settings">The virtual directory settings.</param>
        [CakeMethodAlias]
        public static void AddSiteVirtualDirectory(this ICakeContext context, VirtualDirectorySettings settings)
        {
            context.AddSiteVirtualDirectory("", settings);
        }

        /// <summary>
        /// Adds site virtual directory to remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="settings">The virtual directory settings.</param>
        [CakeMethodAlias]
        public static void AddSiteVirtualDirectory(this ICakeContext context, string server, VirtualDirectorySettings settings)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                settings.ComputerName = server;

                WebsiteManager
                    .Using(context.Environment, context.Log, manager)
                    .AddVirtualDirectory(settings);
            }
        }

        /// <summary>
        /// Removes site virtual directory from local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="settings">The virtual directory settings.</param>
        [CakeMethodAlias]
        public static void RemoveSiteVirtualDirectory(this ICakeContext context, VirtualDirectorySettings settings)
        {
            context.RemoveSiteVirtualDirectory("", settings);
        }

        /// <summary>
        /// Removes site virtual directory from remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="settings">The virtual directory settings.</param>
        [CakeMethodAlias]
        public static void RemoveSiteVirtualDirectory(this ICakeContext context, string server, VirtualDirectorySettings settings)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                WebsiteManager
                    .Using(context.Environment, context.Log, manager)
                    .RemoveVirtualDirectory(settings);
            }
        }

        /// <summary>
        /// Checks if site virtual directory exists in remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="settings">The virtual directory settings.</param>
        [CakeMethodAlias]
        public static bool SiteVirtualDirectoryExists(this ICakeContext context, string server, VirtualDirectorySettings settings)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                return WebsiteManager
                    .Using(context.Environment, context.Log, manager)
                    .VirtualDirectoryExists(settings);
            }
        }
    }
}
