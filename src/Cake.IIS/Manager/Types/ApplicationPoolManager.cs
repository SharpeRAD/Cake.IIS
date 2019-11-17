#region Using Statements
using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Web.Administration;

using Cake.Core;
using Cake.Core.Diagnostics;
#endregion



namespace Cake.IIS
{
    /// <summary>
    /// Class for managing application pools
    /// </summary>
    public class ApplicationPoolManager : BaseManager
    {
        #region Fields
        private static readonly string[] ApplicationPoolBlackList =
        {
                "DefaultAppPool",
                "Classic .NET AppPool",
                "ASP.NET v4.0 Classic",
                "ASP.NET v4.0"
        };
        #endregion





        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationPoolManager" /> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="log">The log.</param>
        public ApplicationPoolManager(ICakeEnvironment environment, ICakeLog log)
                : base(environment, log)
        {

        }
        #endregion





        #region Methods
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
        /// <returns>If the application pool was added.</returns>
        public void Create(ApplicationPoolSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (string.IsNullOrWhiteSpace(settings.Name))
            {
                throw new ArgumentException("Application pool name cannot be null!");
            }

            if (this.IsSystemDefault(settings.Name))
            {
                return;
            }



            //Get Pool
            var pool = _Server.ApplicationPools.FirstOrDefault(p => p.Name == settings.Name);

            if (pool != null)
            {
                _Log.Information("Application pool '{0}' already exists.", settings.Name);

                if (settings.Overwrite)
                {
                    _Log.Information("Application pool '{0}' will be overriden by request.", settings.Name);
                    this.Delete(settings.Name);
                }
                else return;
            }



            //Add Pool
            pool = _Server.ApplicationPools.Add(settings.Name);

            pool.AutoStart = settings.Autostart;

            pool.Enable32BitAppOnWin64 = settings.Enable32BitAppOnWin64;
            pool.ManagedRuntimeVersion = settings.ManagedRuntimeVersion;
            pool.ManagedPipelineMode = settings.ClassicManagedPipelineMode
                ? ManagedPipelineMode.Classic
                : ManagedPipelineMode.Integrated;



            //Set Identity
            _Log.Information("Application pool identity type: {0}", settings.IdentityType.ToString());

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



            _Server.CommitChanges();

            _Log.Information("Application pool created.");
        }

        /// <summary>
        /// Delete an application pool
        /// </summary>
        /// <param name="name">The name of the application pool</param>
        /// <returns>If the application pool was deleted.</returns>
        public bool Delete(string name)
        {
            if (!this.IsSystemDefault(name))
            {
                var pool = _Server.ApplicationPools.FirstOrDefault(p => p.Name == name);

                if (pool == null)
                {
                    _Log.Information("Application pool '{0}' not found.", name);
                    return false;
                }
                else
                {
                    _Server.ApplicationPools.Remove(pool);
                    _Server.CommitChanges();

                    _Log.Information("Application pool '{0}' deleted.", pool.Name);
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Recycle an application pool
        /// </summary>
        /// <param name="name">The name of the application pool</param>
        /// <returns>If the application pool was recycled.</returns>
        public bool Recycle(string name)
        {
            var pool = _Server.ApplicationPools.FirstOrDefault(p => p.Name == name);

            if (pool == null)
            {
                _Log.Information("Application pool '{0}' not found.", name);
                return false;
            }
            else
            {
                try
                {
                    pool.Recycle();
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    _Log.Information("Waiting for IIS to activate new config");
                    Task.Delay(1000).Wait();
                }

                _Log.Information("Application pool '{0}' recycled.", pool.Name);
                return true;
            }
        }

        /// <summary>
        /// Start an application pool
        /// </summary>
        /// <param name="name">The name of the application pool</param>
        /// <returns>If the application pool was started.</returns>
        public bool Start(string name)
        {
            var pool = _Server.ApplicationPools.FirstOrDefault(p => p.Name == name);

            if (pool == null)
            {
                _Log.Information("Application pool '{0}' not found.", name);
                return false;
            }
            else
            {
                if (pool.State == ObjectState.Started)
                {
                    _Log.Information($"Pool '{name}' already started");
                    return true;
                }

                try
                {
                    pool.Start();
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    _Log.Information($"Error on starting pool {name}");
                    throw;
                }

                var timeout = 30;

                while (pool.State != ObjectState.Started && timeout-- > 0)
                {
                    Task.Delay(1000).Wait();
                }

                if (pool.State != ObjectState.Started)
                {
                    var msg = $"Timeout on starting pool {name}";
                    _Log.Information(msg);
                    throw new Exception(msg);
                }

                _Log.Information("Application pool '{0}' started.", pool.Name);
                return true;
            }
        }

        /// <summary>
        /// Stops an application pool
        /// </summary>
        /// <param name="name">The name of the application pool</param>
        /// <returns>If the application pool was stopped.</returns>
        public bool Stop(string name)
        {
            var pool = _Server.ApplicationPools.FirstOrDefault(p => p.Name == name);

            if (pool == null)
            {
                _Log.Information("Application pool '{0}' not found.", name);
                return false;
            }
            else
            {

                if (pool.State == ObjectState.Stopped)
                {
                    _Log.Information($"Pool '{name}' already stopped");
                    return true;
                }

                try
                {
                    pool.Stop();
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    _Log.Information($"Error on stopping pool {name}");
                    throw;
                }

                var timeout = 30;

                while (pool.State != ObjectState.Stopped && timeout-- > 0)
                {
                    Task.Delay(1000).Wait();
                }

                if (pool.State != ObjectState.Stopped)
                {
                    var msg = $"Timeout on stopping pool {name}";
                    _Log.Information(msg);
                    throw new Exception(msg);
                }

                _Log.Information("Application pool '{0}' stopped.", pool.Name);
                return true;
            }
        }

        /// <summary>
        /// Checks if an application pool exists
        /// </summary>
        /// <param name="name">The name of the application pool</param>
        /// <returns>If the application pool exists.</returns>
        public bool Exists(string name)
        {
            if (_Server.ApplicationPools.SingleOrDefault(p => p.Name == name) != null)
            {
                _Log.Information("The ApplicationPool '{0}' exists.", name);
                return true;
            }
            else
            {
                _Log.Information("The ApplicationPool '{0}' does not exist.", name);
                return false;
            }
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
                _Log.Information("Application pool '{0}' is system's default.", name);
                return true;
            }

            return false;
        }
        #endregion
    }
}