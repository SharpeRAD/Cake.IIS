#region Using Statements
using System;

using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Diagnostics;

using Microsoft.Web.Administration;
#endregion



namespace Cake.IIS
{
    /// <summary>
    /// Base class for managing IIS
    /// </summary>
    public abstract class BaseManager
    {
        #region Fields (3)
        /// <summary>
        /// Cake environment
        /// </summary>
        protected readonly ICakeEnvironment _Environment;

        /// <summary>
        /// Cake log
        /// </summary>
        protected readonly ICakeLog _Log;
        #endregion





        #region Constructors (1)
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseManager" /> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="log">The log.</param>
        public BaseManager(ICakeEnvironment environment, ICakeLog log)
        {
            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }

            _Environment = environment;
            _Log = log;
        }
        #endregion

        #region Properties
        /// <summary>
        /// IIS server manager
        /// </summary>
        public ServerManager ServerManager { get; private set; }
        #endregion


        #region Methods (5)
        /// <summary>
        /// Creates a IIS ServerManager
        /// </summary>
        /// <param name="server">The name of the server to connect to.</param>
        /// <returns>IIS ServerManager.</returns>
        public static ServerManager Connect(string server)
        {
            if (String.IsNullOrEmpty(server))
            {
                return new ServerManager();
            }
            else
            {
                return ServerManager.OpenRemote(server);
            }
        }



        /// <summary>
        /// Set the IIS ServerManager
        /// </summary>
        public void SetServer()
        {
            this.SetServer(BaseManager.Connect(""));
        }

        /// <summary>
        /// Set the IIS ServerManager
        /// </summary>
        /// <param name="server">The name of the server to connect to.</param>
        public void SetServer(string server)
        {
            this.SetServer(BaseManager.Connect(server));
        }

        /// <summary>
        /// Set the IIS ServerManager
        /// </summary>
        /// <param name="manager">The manager to connect to.</param>
        public void SetServer(ServerManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            ServerManager = manager;
        }



        /// <summary>
        /// Gets the physical directory from the working directory
        /// </summary>
        /// <param name="settings">The directory settings.</param>
        /// <returns>The directory path.</returns>
        protected string GetPhysicalDirectory(IDirectorySettings settings)
        {
            if (String.IsNullOrEmpty(settings.ComputerName))
            {
                DirectoryPath workingDirectory = settings.WorkingDirectory ?? _Environment.WorkingDirectory;

                settings.WorkingDirectory = workingDirectory.MakeAbsolute(_Environment);
            }
            else if (settings.WorkingDirectory == null)
            {
                settings.WorkingDirectory = new DirectoryPath("C:/");
            }

            var path = settings.PhysicalDirectory.MakeAbsolute(settings.WorkingDirectory).FullPath;
            return path.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
        }
        #endregion
    }
}