using System;
using System.Linq;
using System.Threading;
using Microsoft.Web.Administration;
using Cake.Core;
using Cake.Core.Diagnostics;

namespace Cake.IIS
{
    /// <summary>
    /// Class for managing application pools
    /// </summary>
    public class ApplicationPoolManager : BaseManager
    {
        private static readonly string[] ApplicationPoolBlackList =
        {
            "DefaultAppPool",
            "Classic .NET AppPool",
            "ASP.NET v4.0 Classic",
            "ASP.NET v4.0"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationPoolManager" /> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="log">The log.</param>
        public ApplicationPoolManager(ICakeEnvironment environment, ICakeLog log)
                : base(environment, log)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ApplicationPoolManager" /> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="log">The log.</param>
        /// <param name="server">The <see cref="ServerManager" /> to connect to.</param>
        /// <returns>a new instance of the <see cref="ApplicationPoolManager" />.</returns>
        public static ApplicationPoolManager Using(ICakeEnvironment environment, ICakeLog log, ServerManager server)
        {
            ApplicationPoolManager manager = new ApplicationPoolManager(environment, log);
            manager.SetServer(server);
            return manager;
        }

        /// <summary>
        /// Creates an application pool
        /// </summary>
        /// <param name="settings">The settings of the application to add</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns>If the application pool was added.</returns>
        public void Create(ApplicationPoolSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(settings.Name))
            {
                throw new ArgumentException("Application pool name cannot be null!");
            }

            if (IsSystemDefault(settings.Name))
            {
                return;
            }

            //Get Pool
            var pool = Server.ApplicationPools.FirstOrDefault(p => p.Name == settings.Name);
            if (pool != null)
            {
                Log.Information("Application pool '{0}' already exists.", settings.Name);

                if (settings.Overwrite)
                {
                    Log.Information("Application pool '{0}' will be overriden by request.", settings.Name);
                    Delete(settings.Name);
                }
                else return;
            }

            //Add Pool
            pool = Server.ApplicationPools.Add(settings.Name);

            pool.AutoStart = settings.Autostart;

            pool.Enable32BitAppOnWin64 = settings.Enable32BitAppOnWin64;
            pool.ManagedRuntimeVersion = settings.ManagedRuntimeVersion;
            pool.ManagedPipelineMode = settings.ClassicManagedPipelineMode
                ? ManagedPipelineMode.Classic
                : ManagedPipelineMode.Integrated;

            //Set Identity
            Log.Information("Application pool identity type: {0}", settings.IdentityType.ToString());

            switch (settings.IdentityType)
            {
                case IdentityType.LocalSystem:
                    pool.ProcessModel.IdentityType = ProcessModelIdentityType.LocalSystem;
                    break;
                case IdentityType.LocalService:
                    pool.ProcessModel.IdentityType = ProcessModelIdentityType.LocalService;
                    break;
                case IdentityType.NetworkService:
                    pool.ProcessModel.IdentityType = ProcessModelIdentityType.NetworkService;
                    break;
                case IdentityType.ApplicationPoolIdentity:
                    pool.ProcessModel.IdentityType = ProcessModelIdentityType.ApplicationPoolIdentity;
                    break;
                case IdentityType.SpecificUser:
                    pool.ProcessModel.IdentityType = ProcessModelIdentityType.SpecificUser;
                    pool.ProcessModel.UserName = settings.Username;
                    pool.ProcessModel.Password = settings.Password;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //Set ProcessModel
            pool.ProcessModel.LoadUserProfile = settings.LoadUserProfile;
            pool.ProcessModel.MaxProcesses = settings.MaxProcesses;

            pool.ProcessModel.PingingEnabled = settings.PingingEnabled;

            if (settings.PingResponseTime != TimeSpan.MinValue)
            {
                pool.ProcessModel.PingInterval = settings.PingInterval;
            }
            if (settings.PingResponseTime != TimeSpan.MinValue)
            {
                pool.ProcessModel.PingResponseTime = settings.PingResponseTime;
            }
            if (settings.IdleTimeout != TimeSpan.MinValue)
            {
                pool.ProcessModel.IdleTimeout = settings.IdleTimeout;
            }
            if (settings.ShutdownTimeLimit != TimeSpan.MinValue)
            {
                pool.ProcessModel.ShutdownTimeLimit = settings.ShutdownTimeLimit;
            }
            if (settings.StartupTimeLimit != TimeSpan.MinValue)
            {
                pool.ProcessModel.StartupTimeLimit = settings.StartupTimeLimit;
            }

            Server.CommitChanges();

            Log.Information("Application pool created.");
        }

        /// <summary>
        /// Delete an application pool
        /// </summary>
        /// <param name="name">The name of the application pool</param>
        /// <returns>If the application pool was deleted.</returns>
        public bool Delete(string name)
        {
            if (!IsSystemDefault(name))
            {
                var pool = Server.ApplicationPools.FirstOrDefault(p => p.Name == name);

                if (pool == null)
                {
                    Log.Information("Application pool '{0}' not found.", name);
                    return false;
                }
                Server.ApplicationPools.Remove(pool);
                Server.CommitChanges();

                Log.Information("Application pool '{0}' deleted.", pool.Name);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Recycle an application pool
        /// </summary>
        /// <param name="name">The name of the application pool</param>
        /// <returns>If the application pool was recycled.</returns>
        public bool Recycle(string name)
        {
            var pool = Server.ApplicationPools.FirstOrDefault(p => p.Name == name);

            if (pool == null)
            {
                Log.Information("Application pool '{0}' not found.", name);
                return false;
            }
            try
            {
                pool.Recycle();
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                Log.Information("Waiting for IIS to activate new config");
                Thread.Sleep(1000);
            }

            Log.Information("Application pool '{0}' recycled.", pool.Name);
            return true;
        }

        /// <summary>
        /// Start an application pool
        /// </summary>
        /// <param name="name">The name of the application pool</param>
        /// <returns>If the application pool was started.</returns>
        public bool Start(string name)
        {
            var pool = Server.ApplicationPools.FirstOrDefault(p => p.Name == name);

            if (pool == null)
            {
                Log.Information("Application pool '{0}' not found.", name);
                return false;
            }
            try
            {
                pool.Start();
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                Log.Information("Waiting for IIS to activate new config");
                Thread.Sleep(1000);
            }

            Log.Information("Application pool '{0}' started.", pool.Name);
            return true;
        }

        /// <summary>
        /// Stops an application pool
        /// </summary>
        /// <param name="name">The name of the application pool</param>
        /// <returns>If the application pool was stopped.</returns>
        public bool Stop(string name)
        {
            var pool = Server.ApplicationPools.FirstOrDefault(p => p.Name == name);

            if (pool == null)
            {
                Log.Information("Application pool '{0}' not found.", name);
                return false;
            }
            try
            {
                pool.Stop();
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                Log.Information("Waiting for IIS to activate new config");
                Thread.Sleep(1000);
            }

            Log.Information("Application pool '{0}' stopped.", pool.Name);
            return true;
        }

        /// <summary>
        /// Checks if an application pool exists
        /// </summary>
        /// <param name="name">The name of the application pool</param>
        /// <returns>If the application pool exists.</returns>
        public bool Exists(string name)
        {
            if (Server.ApplicationPools.SingleOrDefault(p => p.Name == name) != null)
            {
                Log.Information("The ApplicationPool '{0}' exists.", name);
                return true;
            }
            Log.Information("The ApplicationPool '{0}' does not exist.", name);
            return false;
        }

        /// <summary>
        /// Checks if an application pool has a default name
        /// </summary>
        /// <param name="name">The name of the application pool</param>
        /// <returns>If the application pool has a default name.</returns>
        public bool IsSystemDefault(string name)
        {
            if (ApplicationPoolBlackList.Contains(name))
            {
                return true;
            }

            Log.Information("Application pool '{0}' is system's default.", name);
            return false;
        }
    }
}