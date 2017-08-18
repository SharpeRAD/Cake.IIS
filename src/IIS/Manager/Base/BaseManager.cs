using System;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Diagnostics;
using Microsoft.Web.Administration;

namespace Cake.IIS
{
    /// <summary>
    /// Base class for managing IIS
    /// </summary>
    public abstract class BaseManager
    {
        /// <summary>
        /// Cake environment
        /// </summary>
        protected readonly ICakeEnvironment Environment;

        /// <summary>
        /// Cake log
        /// </summary>
        protected readonly ICakeLog Log;

        /// <summary>
        /// IIS server manager
        /// </summary>
        protected ServerManager Server;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseManager" /> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="log">The log.</param>
        protected BaseManager(ICakeEnvironment environment, ICakeLog log)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            Environment = environment;
            Log = log;
        }

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
            return ServerManager.OpenRemote(server);
        }

        /// <summary>
        /// Set the IIS ServerManager
        /// </summary>
        public void SetServer()
        {
            SetServer(Connect(""));
        }

        /// <summary>
        /// Set the IIS ServerManager
        /// </summary>
        /// <param name="server">The name of the server to connect to.</param>
        public void SetServer(string server)
        {
            SetServer(Connect(server));
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

            Server = manager;
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
                DirectoryPath workingDirectory = settings.WorkingDirectory ?? Environment.WorkingDirectory;

                settings.WorkingDirectory = workingDirectory.MakeAbsolute(Environment);
            }
            else if (settings.WorkingDirectory == null)
            {
                settings.WorkingDirectory = new DirectoryPath("C:/");
            }

            var path = settings.PhysicalDirectory.MakeAbsolute(settings.WorkingDirectory).FullPath;
            return path.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
        }
    }
}