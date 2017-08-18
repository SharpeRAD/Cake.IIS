using Cake.Core;
using Cake.Core.Annotations;
using Microsoft.Web.Administration;

namespace Cake.IIS
{
    /// <summary>
    /// Contains aliases for working with IIS FTP sites.
    /// </summary>
    [CakeAliasCategory("IIS")]
    [CakeNamespaceImport("Microsoft.Web.Administration")]
    public static class FtpsiteAliases
    {
        /// <summary>
        /// Creates new FTP site on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="settings">The new FTP site settings.</param>
        [CakeMethodAlias]
        public static void CreateFtpsite(this ICakeContext context, FtpsiteSettings settings)
        {
            context.CreateFtpsite("", settings);
        }

        /// <summary>
        /// Creates new FTP site on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="settings">The new FTP site settings.</param>
        [CakeMethodAlias]
        public static void CreateFtpsite(this ICakeContext context, string server, FtpsiteSettings settings)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                settings.ComputerName = server;

                FtpsiteManager
                    .Using(context.Environment, context.Log, manager)
                    .Create(settings);
            }
        }
    }
}
