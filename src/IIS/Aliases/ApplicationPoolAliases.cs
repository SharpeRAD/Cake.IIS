using Cake.Core;
using Cake.Core.Annotations;
using Microsoft.Web.Administration;

namespace Cake.IIS
{
    /// <summary>
    /// Contains aliases for working with IIS application pools.
    /// </summary>
    [CakeAliasCategory("IIS")]
    [CakeNamespaceImport("Microsoft.Web.Administration")]
    public static class ApplicationPoolAliases
    {
        /// <summary>
        /// Checks if application pool exists on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The application pool name.</param>
        /// <returns><c>true</c> if exists</returns>
        [CakeMethodAlias]
        public static bool PoolExists(this ICakeContext context, string name)
        {
            return context.PoolExists("", name);
        }

        /// <summary>
        /// Checks if application pool exists on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="name">The application pool name.</param>
        /// <returns><c>true</c> if exists</returns>
        [CakeMethodAlias]
        public static bool PoolExists(this ICakeContext context, string server, string name)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                return ApplicationPoolManager
                        .Using(context.Environment, context.Log, manager)
                        .Exists(name);
            }
        }

        /// <summary>
        /// Creates application pool on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="settings">The application pool settings.</param>
        [CakeMethodAlias]
        public static void CreatePool(this ICakeContext context, ApplicationPoolSettings settings)
        {
            context.CreatePool("", settings);
        }

        /// <summary>
        /// Creates application pool on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="settings">The application pool settings.</param>
        [CakeMethodAlias]
        public static void CreatePool(this ICakeContext context, string server, ApplicationPoolSettings settings)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                ApplicationPoolManager
                    .Using(context.Environment, context.Log, manager)
                    .Create(settings);
            }
        }

        /// <summary>
        /// Deletes application pool from local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The application pool name.</param>
        /// <returns><c>true</c> if deleted</returns>
        [CakeMethodAlias]
        public static bool DeletePool(this ICakeContext context, string name)
        {
            return context.DeletePool("", name);
        }

        /// <summary>
        /// Deletes application pool from remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="name">The application pool name.</param>
        /// <returns><c>true</c> if deleted</returns>
        [CakeMethodAlias]
        public static bool DeletePool(this ICakeContext context, string server, string name)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                return ApplicationPoolManager
                        .Using(context.Environment, context.Log, manager)
                        .Delete(name);
            }
        }

        /// <summary>
        /// Starts application pool on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The application pool name.</param>
        /// <returns><c>true</c> if started</returns>
        [CakeMethodAlias]
        public static bool StartPool(this ICakeContext context, string name)
        {
            return context.StartPool("", name);
        }

        /// <summary>
        /// Starts application pool on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="name">The application pool name.</param>
        /// <returns><c>true</c> if started</returns>
        [CakeMethodAlias]
        public static bool StartPool(this ICakeContext context, string server, string name)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                return ApplicationPoolManager
                        .Using(context.Environment, context.Log, manager)
                        .Start(name);
            }
        }

        /// <summary>
        /// Stops application pool on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The application pool name.</param>
        /// <returns><c>true</c> if started</returns>
        [CakeMethodAlias]
        public static bool StopPool(this ICakeContext context, string name)
        {
            return context.StopPool("", name);
        }

        /// <summary>
        /// Stops application pool on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="name">The application pool name.</param>
        /// <returns><c>true</c> if started</returns>
        [CakeMethodAlias]
        public static bool StopPool(this ICakeContext context, string server, string name)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                return ApplicationPoolManager
                        .Using(context.Environment, context.Log, manager)
                        .Stop(name);
            }
        }

        /// <summary>
        /// Restarts application pool on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The application pool name.</param>
        /// <returns><c>true</c> if started</returns>
        [CakeMethodAlias]
        public static bool RestartPool(this ICakeContext context, string name)
        {
            return context.RestartPool("", name);
        }

        /// <summary>
        /// Restarts application pool on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="name">The application pool name.</param>
        /// <returns><c>true</c> if started</returns>
        [CakeMethodAlias]
        public static bool RestartPool(this ICakeContext context, string server, string name)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                ApplicationPoolManager poolManager = ApplicationPoolManager.Using(context.Environment, context.Log, manager);

                if (poolManager.Stop(name))
                {
                    return poolManager.Start(name);
                }
                return false;
            }
        }

        /// <summary>
        /// Recycles application pool on local IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="name">The application pool name.</param>
        /// <returns><c>true</c> if started</returns>
        [CakeMethodAlias]
        public static bool RecyclePool(this ICakeContext context, string name)
        {
            return context.RecyclePool("", name);
        }

        /// <summary>
        /// Recycles application pool on remote IIS.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The remote server name.</param>
        /// <param name="name">The application pool name.</param>
        /// <returns><c>true</c> if started</returns>
        [CakeMethodAlias]
        public static bool RecyclePool(this ICakeContext context, string server, string name)
        {
            using (ServerManager manager = BaseManager.Connect(server))
            {
                return ApplicationPoolManager
                        .Using(context.Environment, context.Log, manager)
                        .Recycle(name);
            }
        }
    }
}
