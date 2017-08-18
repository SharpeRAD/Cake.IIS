using Cake.Core;
using Cake.Core.Annotations;
using Microsoft.Web.Administration;

namespace Cake.IIS
{
    /// <summary>
    /// Contains aliases for working with IIS bindings
    /// </summary>
    [CakeAliasCategory("IIS")]
    [CakeNamespaceImport("Microsoft.Web.Administration")]
    public static class BindingAliases
    {
        /// <summary>
        /// Add site binding to local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="siteName">The site name.</param>
        /// <param name="settings">The binding settings.</param>
        [CakeMethodAlias]
        public static void AddBinding(this ICakeContext context, string siteName, BindingSettings settings)
        {
            context.AddBinding("", siteName, settings);
        }

        /// <summary>
        /// Add site binding to remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="siteName">The site name.</param>
        /// <param name="settings">The binding settings.</param>
        [CakeMethodAlias]
        public static void AddBinding(this ICakeContext context, string server, string siteName, BindingSettings settings)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                WebsiteManager
                    .Using(context.Environment, context.Log, manager)
                    .AddBinding(siteName, settings);
            }
        }

        /// <summary>
        /// Removes site binding from local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="siteName">The site name.</param>
        /// <param name="settings">The binding settings.</param>
        [CakeMethodAlias]
        public static void RemoveBinding(this ICakeContext context, string siteName, BindingSettings settings)
        {
            context.RemoveBinding("", siteName, settings);
        }

        /// <summary>
        /// Removes site binding from remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="siteName">The site name.</param>
        /// <param name="settings">The binding settings.</param>
        [CakeMethodAlias]
        public static void RemoveBinding(this ICakeContext context, string server, string siteName, BindingSettings settings)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                WebsiteManager
                    .Using(context.Environment, context.Log, manager)
                    .RemoveBinding(siteName, settings);
            }
        }
    }
}
