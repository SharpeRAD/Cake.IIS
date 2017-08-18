using Cake.Core;
using Cake.Core.Annotations;
using Microsoft.Web.Administration;

namespace Cake.IIS
{
    /// <summary>
    /// Contains aliases for working with IIS web farms.
    /// </summary>
    [CakeAliasCategory("IIS")]
    [CakeNamespaceImport("Microsoft.Web.Administration")]
    public static class WebFarmAliases
    {
        /// <summary>
        /// Checks if web farm exists on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The web site name.</param>
        /// <returns><c>true</c> if exists</returns>
        [CakeMethodAlias]
        public static bool WebFarmExists(this ICakeContext context, string name)
        {
            return context.WebFarmExists("", name);
        }

        /// <summary>
        /// Checks if web farm exists on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="name">The web site name.</param>
        /// <returns><c>true</c> if exists</returns>
        [CakeMethodAlias]
        public static bool WebFarmExists(this ICakeContext context, string server, string name)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                return WebFarmManager
                        .Using(context.Environment, context.Log, manager)
                        .Exists(name);
            }
        }

        /// <summary>
        /// Creates a web farm on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="settings">The web farm settings.</param>
        [CakeMethodAlias]
        public static void CreateWebFarm(this ICakeContext context, WebFarmSettings settings)
        {
            context.CreateWebFarm("", settings);
        }

        /// <summary>
        /// Creates a web farm on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="settings">The web farm settings.</param>
        [CakeMethodAlias]
        public static void CreateWebFarm(this ICakeContext context, string server, WebFarmSettings settings)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                WebFarmManager
                    .Using(context.Environment, context.Log, manager)
                    .Create(settings);
            }
        }

        /// <summary>
        /// Deletes a web farm from local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The web site name.</param>
        /// <returns><c>true</c> if exists</returns>
        [CakeMethodAlias]
        public static bool DeleteWebFarm(this ICakeContext context, string name)
        {
            return context.DeleteWebFarm("", name);
        }

        /// <summary>
        /// Deletes a web farm from remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="name">The web site name.</param>
        /// <returns><c>true</c> if exists</returns>
        [CakeMethodAlias]
        public static bool DeleteWebFarm(this ICakeContext context, string server, string name)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                return WebFarmManager
                        .Using(context.Environment, context.Log, manager)
                        .Delete(name);
            }
        }

        /// <summary>
        /// Add a server to a web farm on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="farm">The web farm name.</param>
        /// <param name="address">The server address.</param>
        /// <returns><c>true</c> if added</returns>
        [CakeMethodAlias]
        public static bool AddServer(this ICakeContext context, string farm, string address)
        {
            return context.AddServer("", farm, address);
        }

        /// <summary>
        /// Add a server to a web farm on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote IIS server name.</param>
        /// <param name="farm">The web farm name.</param>
        /// <param name="address">The server address.</param>
        /// <returns><c>true</c> if added</returns>
        [CakeMethodAlias]
        public static bool AddServer(this ICakeContext context, string server, string farm, string address)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                return WebFarmManager
                        .Using(context.Environment, context.Log, manager)
                        .AddServer(farm, address);
            }
        }

        /// <summary>
        /// Remove a server to from web farm on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="farm">The web farm name.</param>
        /// <param name="address">The server address.</param>
        /// <returns><c>true</c> if removed</returns>
        [CakeMethodAlias]
        public static bool RemoveServer(this ICakeContext context, string farm, string address)
        {
            return context.RemoveServer("", farm, address);
        }

        /// <summary>
        /// Remove a server from a web farm on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote IIS server name.</param>
        /// <param name="farm">The web farm name.</param>
        /// <param name="address">The server address.</param>
        /// <returns><c>true</c> if removed</returns>
        [CakeMethodAlias]
        public static bool RemoveServer(this ICakeContext context, string server, string farm, string address)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                return WebFarmManager
                        .Using(context.Environment, context.Log, manager)
                        .RemoveServer(farm, address);
            }
        }



        /// <summary>
        /// Checks if server exists in a web farm on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="farm">The web farm name.</param>
        /// <param name="address">The server address.</param>
        /// <returns><c>true</c> if removed</returns>
        [CakeMethodAlias]
        public static bool ServerExists(this ICakeContext context, string farm, string address)
        {
            return context.ServerExists("", farm, address);
        }

        /// <summary>
        /// Checks if server exists in a web farm on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote IIS server name.</param>
        /// <param name="farm">The web farm name.</param>
        /// <param name="address">The server address.</param>
        /// <returns><c>true</c> if removed</returns>
        [CakeMethodAlias]
        public static bool ServerExists(this ICakeContext context, string server, string farm, string address)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                return WebFarmManager
                        .Using(context.Environment, context.Log, manager)
                        .ServerExists(farm, address);
            }
        }

        /// <summary>
        /// Sets server to healthy in a web farm on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="farm">The web farm name.</param>
        /// <param name="address">The server address.</param>
        [CakeMethodAlias]
        public static void SetServerHealthy(this ICakeContext context, string farm, string address)
        {
            context.SetServerHealthy("", farm, address);
        }

        /// <summary>
        /// Sets server to healthy in a web farm on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote IIS server name.</param>
        /// <param name="farm">The web farm name.</param>
        /// <param name="address">The server address.</param>
        [CakeMethodAlias]
        public static void SetServerHealthy(this ICakeContext context, string server, string farm, string address)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                WebFarmManager
                    .Using(context.Environment, context.Log, manager)
                    .SetServerHealthy(farm, address);
            }
        }

        /// <summary>
        /// Sets server to unhealthy in a web farm on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="farm">The web farm name.</param>
        /// <param name="address">The server address.</param>
        [CakeMethodAlias]
        public static void SetServerUnhealthy(this ICakeContext context, string farm, string address)
        {
            context.SetServerUnhealthy("", farm, address);
        }

        /// <summary>
        /// Sets server to unhealthy in a web farm on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote IIS server name.</param>
        /// <param name="farm">The web farm name.</param>
        /// <param name="address">The server address.</param>
        [CakeMethodAlias]
        public static void SetServerUnhealthy(this ICakeContext context, string server, string farm, string address)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                WebFarmManager
                    .Using(context.Environment, context.Log, manager)
                    .SetServerUnhealthy(farm, address);
            }
        }

        /// <summary>
        /// Sets server to available in a web farm on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="farm">The web farm name.</param>
        /// <param name="address">The server address.</param>
        [CakeMethodAlias]
        public static void SetServerAvailable(this ICakeContext context, string farm, string address)
        {
            context.SetServerAvailable("", farm, address);
        }

        /// <summary>
        /// Sets server to available in a web farm on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote IIS server name.</param>
        /// <param name="farm">The web farm name.</param>
        /// <param name="address">The server address.</param>
        [CakeMethodAlias]
        public static void SetServerAvailable(this ICakeContext context, string server, string farm, string address)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                WebFarmManager
                    .Using(context.Environment, context.Log, manager)
                    .SetServerAvailable(farm, address);
            }
        }

        /// <summary>
        /// Sets server to unavailable in a web farm on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="farm">The web farm name.</param>
        /// <param name="address">The server address.</param>
        [CakeMethodAlias]
        public static void SetServerUnavailable(this ICakeContext context, string farm, string address)
        {
            context.SetServerUnavailable("", farm, address);
        }

        /// <summary>
        /// Sets server to unavailable in a web farm on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote IIS server name.</param>
        /// <param name="farm">The web farm name.</param>
        /// <param name="address">The server address.</param>
        [CakeMethodAlias]
        public static void SetServerUnavailable(this ICakeContext context, string server, string farm, string address)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                WebFarmManager
                    .Using(context.Environment, context.Log, manager)
                    .SetServerUnavailable(farm, address);
            }
        }

        /// <summary>
        /// Gets if server is healthy in a web farm on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="farm">The web farm name.</param>
        /// <param name="address">The server address.</param>
        /// <returns><c>true</c> if healthy</returns>
        [CakeMethodAlias]
        public static bool GetServerIsHealthy(this ICakeContext context, string farm, string address)
        {
            return context.GetServerIsHealthy("", farm, address);
        }

        /// <summary>
        /// Gets if server is healthy in a web farm on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote IIS server name.</param>
        /// <param name="farm">The web farm name.</param>
        /// <param name="address">The server address.</param>
        /// <returns><c>true</c> if healthy</returns>
        [CakeMethodAlias]
        public static bool GetServerIsHealthy(this ICakeContext context, string server, string farm, string address)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                return WebFarmManager
                        .Using(context.Environment, context.Log, manager)
                        .GetServerIsHealthy(farm, address);
            }
        }

        /// <summary>
        /// Gets server state in a web farm on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="farm">The web farm name.</param>
        /// <param name="address">The server address.</param>
        /// <returns>state of server</returns>
        [CakeMethodAlias]
        public static string GetServerState(this ICakeContext context, string farm, string address)
        {
            return context.GetServerState("", farm, address);
        }

        /// <summary>
        /// Gets server state in a web farm on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote IIS server name.</param>
        /// <param name="farm">The web farm name.</param>
        /// <param name="address">The server address.</param>
        /// <returns>state of server</returns>
        [CakeMethodAlias]
        public static string GetServerState(this ICakeContext context, string server, string farm, string address)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                return WebFarmManager
                        .Using(context.Environment, context.Log, manager)
                        .GetServerState(farm, address);
            }
        }
    }
}
