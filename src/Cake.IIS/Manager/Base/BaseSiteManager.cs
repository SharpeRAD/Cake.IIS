#region Using Statements
using System;
using System.Linq;
using System.Threading.Tasks;

using Cake.Core;
using Cake.Core.Diagnostics;

using Microsoft.Web.Administration;
#endregion



namespace Cake.IIS
{
    /// <summary>
    /// Base class for managing IIS sites
    /// </summary>
    public abstract class BaseSiteManager : BaseManager
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSiteManager" /> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="log">The log.</param>
        public BaseSiteManager(ICakeEnvironment environment, ICakeLog log)
            : base(environment, log)
        {

        }
        #endregion





        #region Methods
        /// <summary>
        /// Creates a IIS site
        /// </summary>
        /// <param name="settings">The setting of the site</param>
        /// <param name="exists">Check if the site exists</param>
        /// <returns>IIS Site.</returns>
        protected Site CreateSite(SiteSettings settings, out bool exists)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (string.IsNullOrWhiteSpace(settings.Name))
            {
                throw new ArgumentException("Site name cannot be null!");
            }



            //Get Site
            Site site = _Server.Sites.FirstOrDefault(p => p.Name == settings.Name);

            if (site != null)
            {
                _Log.Information("Site '{0}' already exists.", settings.Name);

                if (settings.Overwrite)
                {
                    _Log.Information("Site '{0}' will be overriden by request.", settings.Name);

                    this.Delete(settings.Name);

                    ApplicationPoolManager
                        .Using(_Environment, _Log, _Server)
                        .Delete(site.ApplicationDefaults.ApplicationPoolName);

                    exists = false;
                }
                else
                {
                    exists = true;
                    return site;
                }
            }
            else
            {
                exists = false;
            }



            //Create Pool
            ApplicationPoolManager
                .Using(_Environment, _Log, _Server)
                .Create(settings.ApplicationPool);



            //Site Settings
            site = _Server.Sites.Add(
                settings.Name,
                this.GetPhysicalDirectory(settings),
                settings.Binding.Port);


            if (!String.IsNullOrEmpty(settings.AlternateEnabledProtocols))
            {
                site.ApplicationDefaults.EnabledProtocols = settings.AlternateEnabledProtocols;
            }

            site.Bindings.Clear();
            var binding = site.Bindings.CreateElement();

            binding.Protocol = settings.Binding.BindingProtocol.ToString().ToLower();
            binding.BindingInformation = settings.Binding.BindingInformation;


            if (settings.Binding.CertificateHash != null)
            {
                binding.CertificateHash = settings.Binding.CertificateHash;
            }

            if (!String.IsNullOrEmpty(settings.Binding.CertificateStoreName))
            {
                binding.CertificateStoreName = settings.Binding.CertificateStoreName;
            }

            if (settings.Binding.RequireServerNameIndication)
            {
                if (!string.Equals(settings.Binding.BindingProtocol.ToString(), BindingProtocol.Https.ToString(), StringComparison.Ordinal))
                    throw new Exception("Require Server Name Indication (SNI) is only applicable for HTTPS bindings");

                binding.SslFlags |= SslFlags.Sni;
            }

            site.Bindings.Add(binding);

            site.ServerAutoStart = settings.ServerAutoStart;
            site.ApplicationDefaults.ApplicationPoolName = settings.ApplicationPool.Name;

            // Security
            var serverType = settings is WebsiteSettings ? "webServer" : "ftpServer";
            var hostConfig = GetWebConfiguration();

            hostConfig.SetAuthentication(serverType, settings.Name, "", settings.Authentication, _Log);
            hostConfig.SetAuthorization(serverType, settings.Name, "", settings.Authorization);

            return site;
        }

        /// <summary>
        /// Delets a site from IIS
        /// </summary>
        /// <param name="name">The name of the site to delete</param>
        /// <returns>If the site was deleted.</returns>
        public bool Delete(string name)
        {
            var site = _Server.Sites.FirstOrDefault(p => p.Name == name);

            if (site == null)
            {
                _Log.Information("Site '{0}' not found.", name);
                return true;
            }
            else
            {
                _Server.Sites.Remove(site);
                _Server.CommitChanges();

                _Log.Information("Site '{0}' deleted.", site.Name);
                return false;
            }
        }

        /// <summary>
        /// Checks if a site exists in IIS
        /// </summary>
        /// <param name="name">The name of the site to check</param>
        /// <returns>If the site exists.</returns>
        public bool Exists(string name)
        {
            if (_Server.Sites.SingleOrDefault(p => p.Name == name) != null)
            {
                _Log.Information("The site '{0}' exists.", name);
                return true;
            }
            else
            {
                _Log.Information("The site '{0}' does not exist.", name);
                return false;
            }
        }



        /// <summary>
        /// Starts a IIS site
        /// </summary>
        /// <param name="name">The name of the site to start</param>
        /// <returns>If the site was started.</returns>
        public bool Start(string name)
        {
            var site = _Server.Sites.FirstOrDefault(p => p.Name == name);

            if (site == null)
            {
                _Log.Information("Site '{0}' not found.", name);
                return false;
            }
            else
            {
                try
                {
                    site.Start();
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    _Log.Information("Waiting for IIS to activate new config");
                    Task.Delay(1000).Wait();
                }

                _Log.Information("Site '{0}' started.", site.Name);
                return true;
            }
        }

        /// <summary>
        /// Stops a IIS site
        /// </summary>
        /// <param name="name">The name of the site to stop</param>
        /// <returns>If the site was stopped.</returns>
        public bool Stop(string name)
        {
            var site = _Server.Sites.FirstOrDefault(p => p.Name == name);

            if (site == null)
            {
                _Log.Information("Site '{0}' not found.", name);
                return false;
            }
            else
            {
                try
                {
                    site.Stop();
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    _Log.Information("Waiting for IIS to activate new config");
                    Task.Delay(1000).Wait();
                }

                _Log.Information("Site '{0}' stopped.", site.Name);
                return true;
            }
        }



        /// <summary>
        /// Adds a binding to a IIS site
        /// </summary>
        /// <param name="siteName">The website name</param>
        /// <param name="settings">The settings of the binding</param>
        /// <returns>If the binding was added.</returns>
        public bool AddBinding(string siteName, BindingSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (string.IsNullOrWhiteSpace(siteName))
            {
                throw new ArgumentException("Site name cannot be null!");
            }



            //Get Site
            Site site = _Server.Sites.SingleOrDefault(p => p.Name == siteName);

            if (site != null)
            {
                if (site.Bindings.FirstOrDefault(b => (b.Protocol == settings.BindingProtocol.ToString()) && (b.BindingInformation == settings.BindingInformation)) != null)
                {
                    throw new Exception("A binding with the same ip, port and host header already exists.");
                }

                //Add Binding
                Binding newBinding = site.Bindings.CreateElement();

                newBinding.Protocol = settings.BindingProtocol.ToString();
                newBinding.BindingInformation = settings.BindingInformation;

                if (settings.CertificateHash != null)
                {
                    newBinding.CertificateHash = settings.CertificateHash;
                }

                if (!String.IsNullOrEmpty(settings.CertificateStoreName))
                {
                    newBinding.CertificateStoreName = settings.CertificateStoreName;
                }

                if (settings.RequireServerNameIndication)
                {
                    if (!string.Equals(settings.BindingProtocol.ToString(), BindingProtocol.Https.ToString(), StringComparison.Ordinal))
                        throw new Exception("Require Server Name Indication (SNI) is only applicable for HTTPS bindings");

                    newBinding.SslFlags |= SslFlags.Sni;
                }

                site.Bindings.Add(newBinding);
                _Server.CommitChanges();

                _Log.Information("Binding added.");
                return true;
            }
            else
            {
                throw new Exception("Site: " + siteName + " does not exist.");
            }
        }

        /// <summary>
        /// Removes a binding to a IIS site
        /// </summary>
        /// <param name="siteName">The website name</param>
        /// <param name="settings">The settings of the binding</param>
        /// <returns>If the binding was removed.</returns>
        public bool RemoveBinding(string siteName, BindingSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (string.IsNullOrWhiteSpace(siteName))
            {
                throw new ArgumentException("Site name cannot be null!");
            }



            //Get Site
            Site site = _Server.Sites.SingleOrDefault(p => p.Name == siteName);

            if (site != null)
            {
                Binding binding = site.Bindings.FirstOrDefault(b => (b.Protocol == settings.BindingProtocol.ToString()) && (b.BindingInformation == settings.BindingInformation));

                if (binding != null)
                {
                    //Remove Binding
                    site.Bindings.Remove(binding);
                    _Server.CommitChanges();

                    _Log.Information("Binding removed.");
                    return true;
                }
                else
                {
                    _Log.Information("A binding with the same ip, port and host header does not exists.");
                    return false;
                }
            }
            else
            {
                throw new Exception("Site: " + siteName + " does not exist.");
            }
        }



        /// <summary>
        /// Adds a virtual application to a IIS site
        /// </summary>
        /// <param name="settings">The settings of the application to add</param>
        /// <returns>If the application was added.</returns>
        public bool AddApplication(ApplicationSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (string.IsNullOrWhiteSpace(settings.SiteName))
            {
                throw new ArgumentException("Site name cannot be null!");
            }

            if (string.IsNullOrWhiteSpace(settings.ApplicationPath))
            {
                throw new ArgumentException("Applicaiton path cannot be null!");
            }



            //Get Pool
            ApplicationPool appPool = _Server.ApplicationPools.SingleOrDefault(p => p.Name == settings.ApplicationPool);

            if (appPool == null)
            {
                throw new Exception("Application Pool '" + settings.ApplicationPool + "' does not exist.");
            }



            //Get Site
            Site site = _Server.Sites.SingleOrDefault(p => p.Name == settings.SiteName);

            if (site != null)
            {
                //Get Application
                Application app = site.Applications.SingleOrDefault(p => p.Path == settings.ApplicationPath);

                if (app != null)
                {
                    throw new Exception("Application '" + settings.ApplicationPath + "' already exists.");
                }

                app = site.Applications.CreateElement();
                app.Path = settings.ApplicationPath;
                app.ApplicationPoolName = settings.ApplicationPool;

                if (!String.IsNullOrEmpty(settings.AlternateEnabledProtocols))
                {
                    app.EnabledProtocols = settings.AlternateEnabledProtocols;
                }

                //Get Directory
                VirtualDirectory vDir = app.VirtualDirectories.CreateElement();
                // While creating application only ApplicationPath is used by IIS, so we leave default one here.
                vDir.Path = "/";
                vDir.PhysicalPath = this.GetPhysicalDirectory(settings);

                app.VirtualDirectories.Add(vDir);

                // Security
                var serverType = "webServer";
                var hostConfig = GetWebConfiguration();

                hostConfig.SetAuthentication(serverType, settings.SiteName, settings.ApplicationPath, settings.Authentication, _Log);
                hostConfig.SetAuthorization(serverType, settings.SiteName, settings.ApplicationPath, settings.Authorization);

                // Commit
                site.Applications.Add(app);
                _Server.CommitChanges();

                // Settings that need to be modified after the app is created
                var isModified = false;

                if (settings.EnableDirectoryBrowsing)
                {
                    var appConfig = app.GetWebConfiguration();
                    appConfig.EnableDirectoryBrowsing();
                    isModified = true;
                }

                if (isModified)
                {
                    _Server.CommitChanges();
                }

                return true;
            }
            else
            {
                throw new Exception("Site '" + settings.SiteName + "' does not exist.");
            }
        }

        /// <summary>
        /// Removes a virtual application to a IIS site
        /// </summary>
        /// <param name="settings">The settings of the application to remove</param>
        /// <returns>If the application was removed.</returns>
        public bool RemoveApplication(ApplicationSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (string.IsNullOrWhiteSpace(settings.SiteName))
            {
                throw new ArgumentException("Site name cannot be null!");
            }

            if (string.IsNullOrWhiteSpace(settings.ApplicationPath))
            {
                throw new ArgumentException("Applicaiton path cannot be null!");
            }



            //Get Site
            Site site = _Server.Sites.SingleOrDefault(p => p.Name == settings.SiteName);

            if (site != null)
            {
                //Get Application
                Application app = site.Applications.SingleOrDefault(p => p.Path == settings.ApplicationPath);

                if (app == null)
                {
                    throw new Exception("Application '" + settings.ApplicationPath + "' does not exists.");
                }
                else
                {
                    site.Applications.Remove(app);
                    _Server.CommitChanges();

                    return true;
                }
            }
            else
            {
                throw new Exception("Site '" + settings.SiteName + "' does not exist.");
            }
        }



        /// <summary>
        /// Adds a virtual directory to a IIS site
        /// </summary>
        /// <param name="settings">The settings of the virtual directory to add</param>
        /// <returns>If the virtual directory was added.</returns>
        public bool AddVirtualDirectory(VirtualDirectorySettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (string.IsNullOrWhiteSpace(settings.Path))
            {
                throw new ArgumentException("Site name cannot be null!");
            }

            if (string.IsNullOrWhiteSpace(settings.ApplicationPath))
            {
                throw new ArgumentException("Applicaiton path cannot be null!");
            }



            //Get Site
            Site site = _Server.Sites.SingleOrDefault(p => p.Name == settings.SiteName);

            if (site == null)
            {
                throw new Exception("Site '" + settings.SiteName + "' does not exist.");
            }

            //Get Application
            Application app = site.Applications.SingleOrDefault(p => p.Path == settings.ApplicationPath);

            if (app == null)
            {
                throw new Exception("Application '" + settings.ApplicationPath + "' does not exist.");
            }

            if (app.VirtualDirectories.Any(vd => vd.Path == settings.Path))
            {
                throw new Exception("Virtual Directory '" + settings.Path + "' already exists.");
            }

            //Get Directory
            VirtualDirectory vDir = app.VirtualDirectories.CreateElement();
            vDir.Path = settings.Path;
            vDir.PhysicalPath = this.GetPhysicalDirectory(settings);

            app.VirtualDirectories.Add(vDir);

            //this.SetAuthentication("webServer", settings.SiteName, settings.ApplicationPath, settings.Authentication);
            //this.SetAuthorization("webServer", settings.SiteName, settings.ApplicationPath, settings.Authorization);

            _Server.CommitChanges();

            return true;
        }
        /// <summary>
        /// Removes a virtual directory from a IIS site
        /// </summary>
        /// <param name="settings">The settings of the virtual directory to remove</param>
        /// <returns>If the virtual directory was removed.</returns>
        public bool RemoveVirtualDirectory(VirtualDirectorySettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (string.IsNullOrWhiteSpace(settings.SiteName))
            {
                throw new ArgumentException("Site name cannot be null!");
            }

            if (string.IsNullOrWhiteSpace(settings.ApplicationPath))
            {
                throw new ArgumentException("Applicaiton path cannot be null!");
            }



            //Get Site
            Site site = _Server.Sites.SingleOrDefault(p => p.Name == settings.SiteName);

            if (site != null)
            {
                //Get Application
                Application app = site.Applications.SingleOrDefault(p => p.Path == settings.ApplicationPath);

                if (app == null)
                {
                    throw new Exception("Application '" + settings.ApplicationPath + "' does not exist.");
                }
                else
                {

                    VirtualDirectory vd = app.VirtualDirectories.FirstOrDefault(p => p.Path == settings.Path);

                    if (vd == null)
                    {
                        throw new Exception("Virtual directory '" + settings.Path + "' does not exist.");
                    }
                    else
                    {
                        app.VirtualDirectories.Remove(vd);
                        _Server.CommitChanges();

                        return true;
                    }
                }
            }
            else
            {
                throw new Exception("Site '" + settings.SiteName + "' does not exist.");
            }
        }

        /// <summary>
        /// Checks if a virtual directory exists in a IIS site
        /// </summary>
        /// <param name="settings">The settings of the virtual directory to check</param>
        /// <returns>If the virtual directory exists.</returns>
        public bool VirtualDirectoryExists(VirtualDirectorySettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (string.IsNullOrWhiteSpace(settings.SiteName))
            {
                throw new ArgumentException("Site name cannot be null!");
            }

            if (string.IsNullOrWhiteSpace(settings.ApplicationPath))
            {
                throw new ArgumentException("Applicaiton path cannot be null!");
            }



            //Get Site
            Site site = _Server.Sites.SingleOrDefault(p => p.Name == settings.SiteName);

            if (site != null)
            {
                //Get Application
                Application app = site.Applications.SingleOrDefault(p => p.Path == settings.ApplicationPath);

                if (app == null)
                {
                    throw new Exception("Application '" + settings.ApplicationPath + "' does not exist.");
                }
                else
                {
                    VirtualDirectory vd = app.VirtualDirectories.FirstOrDefault(p => p.Path == settings.Path);
                    return vd != null;
                }
            }
            else
            {
                throw new Exception("Site '" + settings.SiteName + "' does not exist.");
            }
        }

        /// <summary>
        /// Checks if a virtual application exists in a IIS site
        /// </summary>
        /// <param name="settings">The settings of the application to remove</param>
        /// <returns>If the application was removed.</returns>
        public bool ApplicationExists(ApplicationSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (string.IsNullOrWhiteSpace(settings.SiteName))
            {
                throw new ArgumentException("Site name cannot be null!");
            }

            if (string.IsNullOrWhiteSpace(settings.ApplicationPath))
            {
                throw new ArgumentException("Applicaiton path cannot be null!");
            }



            //Get Site
            Site site = _Server.Sites.SingleOrDefault(p => p.Name == settings.SiteName);

            if (site != null)
            {
                //Get Application
                return site.Applications.Any(p => p.Path == settings.ApplicationPath);
            }
            else
            {
                throw new Exception("Site '" + settings.SiteName + "' does not exist.");
            }
        }

        /// <summary>
        /// Updates the web.config of the given host/site/application.
        /// </summary>
        /// <param name="siteName">The name of the site.</param>
        /// <param name="applicationPath">The path to the application.</param>
        /// <param name="configurationAction">The action to execute on the <see cref="Configuration"/> object.</param>
        public void SetWebConfiguration(string siteName, string applicationPath, Action<Configuration> configurationAction)
        {
            if (configurationAction == null)
            {
                throw new ArgumentNullException(nameof(configurationAction));
            }

            var config = GetWebConfiguration(siteName, applicationPath);
            configurationAction(config);
            _Server.CommitChanges();
        }

        /// <summary>
        /// Gets the appropriate web.config configuration object.
        /// This can be the ApplicationHostConfiguration, Site WebConfiguration or Application WebConfiguration.
        /// </summary>
        /// <param name="siteName">The name of the site.</param>
        /// <param name="applicationPath">The path to the application.</param>
        /// <returns></returns>
        private Configuration GetWebConfiguration(string siteName = null, string applicationPath = null)
        {
            Configuration config;

            if (siteName == null)
            {
                // No site, so use the ApplicationHostConfiguration
                config = _Server.GetApplicationHostConfiguration();
            }
            else
            {
                // Try finding the site
                var site = _Server.Sites.SingleOrDefault(p => p.Name == siteName);

                if (site == null)
                {
                    throw new Exception($"Site '{siteName}' does not exist.");
                }

                if (applicationPath == null)
                {
                    // No application path, so use the site's WebConfiguration
                    config = site.GetWebConfiguration();
                }
                else
                {
                    // Try finding the application
                    var app = site.Applications.SingleOrDefault(p => p.Path == applicationPath);

                    if (app == null)
                    {
                        throw new Exception($"Application '{applicationPath}' does not exist.");
                    }

                    config = app.GetWebConfiguration();
                }
            }

            return config;
        }
        #endregion
    }
}