using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.IIS.Manager.Types;
using Microsoft.Web.Administration;

namespace Cake.IIS.Aliases
{
    /// <summary>
    /// Contains aliases for working with IIS virtual applications.
    /// </summary>
    [CakeAliasCategory("IIS")]
    [CakeNamespaceImport("Microsoft.Web.Administration")]
    public static class VirtualApplicationAliases
    {
        /// <summary>
        /// Creates virtual application on local IIS
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The server</param>
        /// <param name="settings">Virtual application settings</param>
        [CakeMethodAlias]
        public static void CreateApplication(this ICakeContext context, string server, VirtualApplicationSettings settings)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                VirtualApplicationManager.Using(context.Environment, context.Log, manager)
                    .Create(settings);
            }
        }

        /// <summary>
        /// Checks if virtual application exists
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="server">server</param>
        /// <param name="webSite">Name of the website</param>
        /// <param name="appName">Name of the virtual application</param>
        /// <returns></returns>
        public static bool ApplicationExists(this ICakeContext context, string server, string webSite, string appName)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                return VirtualApplicationManager.Using(context.Environment, context.Log, manager)
                    .Exists(webSite, appName);
            }
        }
    }
}
