using Cake.Core;
using Cake.Core.Annotations;
using Microsoft.Web.Administration;

namespace Cake.IIS
{
    /// <summary>
    /// Contains aliases for working with IIS web sites.
    /// </summary>
    [CakeAliasCategory("IIS")]
    [CakeNamespaceImport("Microsoft.Web.Administration")]
    public static class WebsiteAliases
    {
        /// <summary>
        /// Creates new web site on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="settings">The new web site settings.</param>
        [CakeMethodAlias]
        public static void CreateWebsite(this ICakeContext context, WebsiteSettings settings)
        {
            context.CreateWebsite("", settings);
        }

        /// <summary>
        /// Creates new web site on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="settings">The new web site settings.</param>
        [CakeMethodAlias]
        public static void CreateWebsite(this ICakeContext context, string server, WebsiteSettings settings)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                settings.ComputerName = server;

                WebsiteManager
                    .Using(context.Environment, context.Log, manager)
                    .Create(settings);
            }
        }
    }
}
