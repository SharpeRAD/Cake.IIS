using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Microsoft.Web.Administration;

namespace Cake.IIS.Manager.Types
{
    public class VirtualApplicationManager : BaseManager
    {
        public VirtualApplicationManager(ICakeEnvironment environment, ICakeLog log) : base(environment, log)
        {
            
        }
        public static VirtualApplicationManager Using(ICakeEnvironment environment, ICakeLog log, ServerManager server)
        {
            VirtualApplicationManager manager = new VirtualApplicationManager(environment, log);
            manager.SetServer(server);

            return manager;
        }

        public void Create(VirtualApplicationSettings settings)
        {
            if(settings.ParentWebSite == null)
                throw new ArgumentException("ParentWebSite needs to be set");

            if (settings.Name == null)
                throw new ArgumentException("Name of virtual application needs to be set");

            var site = _Server.Sites.FirstOrDefault(x => x.Name == settings.ParentWebSite);
            if(site == null)
                throw new ArgumentException("Site with name '" + settings.ParentWebSite + "' was not found");

            if (!settings.Name.StartsWith("/"))
                settings.Name = "/" + settings.Name;

            var app = site.Applications.FirstOrDefault(x => x.Path == settings.Name);
            if (app != null)
            {
                _Log.Information("Virtual application '{0}' already created.", settings.Name);
                if (!settings.Overwrite)
                {
                    _Log.Information("Virtual application '{0}' will be overwriten.", settings.Name);
                }
            }
            else
            {
                _Log.Information("Creating or updating application: '{0}'." , settings.Name);

                var appDirectory = new DirectoryPath(settings.PhysicalPath);
                var path = appDirectory.FullPath;
                path = path.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
                app = site.Applications.Add(settings.Name, path);
            }
            if (!String.IsNullOrWhiteSpace(settings.EnabledProtocols))
                app.EnabledProtocols = settings.EnabledProtocols;
            if (!string.IsNullOrWhiteSpace(settings.ApplicationPoolName))
                app.ApplicationPoolName = settings.ApplicationPoolName;

            _Server.CommitChanges();
            _Log.Information("Virtual application created or updated '{0}'.", settings.Name);
        }

        public void Delete(string webSite, string appName)
        {
            ValidateApplication(webSite, appName);

            if (!appName.StartsWith("/"))
                appName = "/" + appName;

            var site = _Server.Sites.FirstOrDefault(x => x.Name == webSite);

            var app = site?.Applications.FirstOrDefault(x => x.Path == appName);
            if (app != null)
            {
                _Log.Information("Deleting virtual application '{0}' for website '{1}'", appName, webSite);
                app.Delete();
                _Server.CommitChanges();
                _Log.Information("Application '{0}' in '{1} deleted", appName, webSite);
            }
            else throw new ArgumentException("Site or virtual application does not exists.");
        }

        /// <summary>
        /// Checks whether the application exists
        /// </summary>
        /// <param name="webSite">Name of the web-site</param>
        /// <param name="appName">Name of virtual application</param>
        /// <returns></returns>
        public bool Exists(string webSite, string appName)
        {
            ValidateApplication(webSite, appName);

            if (!appName.StartsWith("/"))
                appName = "/" + appName;

            var site = _Server.Sites.FirstOrDefault(x => x.Name == webSite);

            var app = site?.Applications.FirstOrDefault(x => x.Path == appName);
            return app != null;
        }

        private static void ValidateApplication(string webSite, string appName)
        {
            if (appName == null)
                throw new ArgumentException("AppName cannot be null");

            if (webSite == null)
                throw new ArgumentException("WebSite cannot be null.");
        }
    }

    public class VirtualApplicationSettings
    {
        public string Name { get; set; }
        public string PhysicalPath { get; set; }
        public string ParentWebSite { get; set; }
        public string ApplicationPoolName { get; set; }
        public string EnabledProtocols { get; set; }
        public bool Overwrite { get; set; }
        
    }
}
