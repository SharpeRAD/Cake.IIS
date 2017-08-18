using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cake.Core;
using Cake.Core.Diagnostics;
using Microsoft.Web.Administration;

namespace Cake.IIS
{
    /// <summary>
    /// Base class for managing IIS sites
    /// </summary>
    public abstract class BaseSiteManager : BaseManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSiteManager" /> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="log">The log.</param>
        protected BaseSiteManager(ICakeEnvironment environment, ICakeLog log)
            : base(environment, log)
        {

        }

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
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(settings.Name))
            {
                throw new ArgumentException("Site name cannot be null!");
            }

            //Get Site
            Site site = Server.Sites.FirstOrDefault(p => p.Name == settings.Name);

            if (site != null)
            {
                Log.Information("Site '{0}' already exists.", settings.Name);

                if (settings.Overwrite)
                {
                    Log.Information("Site '{0}' will be overriden by request.", settings.Name);

                    Delete(settings.Name);

                    ApplicationPoolManager
                        .Using(Environment, Log, Server)
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
                .Using(Environment, Log, Server)
                .Create(settings.ApplicationPool);

            //Site Settings
            site = Server.Sites.Add(
                settings.Name,
                settings.Binding.BindingProtocol.ToString().ToLower(),
                settings.Binding.BindingInformation,
                GetPhysicalDirectory(settings));

            if (!String.IsNullOrEmpty(settings.AlternateEnabledProtocols))
            {
                site.ApplicationDefaults.EnabledProtocols = settings.AlternateEnabledProtocols;
            }

            if (settings.Binding.CertificateHash != null)
            {
                site.Bindings[0].CertificateHash = settings.Binding.CertificateHash;
            }

            if (!String.IsNullOrEmpty(settings.Binding.CertificateStoreName))
            {
                site.Bindings[0].CertificateStoreName = settings.Binding.CertificateStoreName;
            }

            site.ServerAutoStart = settings.ServerAutoStart;
            site.ApplicationDefaults.ApplicationPoolName = settings.ApplicationPool.Name;

            //Security
            string server = "";

            if (settings is WebsiteSettings)
            {
                server = "webServer";
            }
            else
            {
                server = "ftpServer";
            }

            SetAuthentication(server, settings.Name, "", settings.Authentication);
            SetAuthorization(server, settings.Name, "", settings.Authorization);

            return site;
        }

        /// <summary>
        /// Sets the authentication settings for the site
        /// </summary>
        /// /// <param name="server">The atype of server</param>
        /// <param name="site">The name of the site</param>
        /// <param name="appPath">The application path</param>
        /// <param name="settings">The authentication settings</param>
        protected void SetAuthentication(string server, string site, string appPath, AuthenticationSettings settings)
        {
            if (settings != null)
            {
                //Authentication
                var config = Server.GetApplicationHostConfiguration();

                var locationPath = site + appPath;
                var authentication = config.GetSection("system." + server + "/security/authorization", locationPath);

                // Anonymous Authentication
                var anonymousAuthentication = authentication.GetChildElement("anonymousAuthentication");

                anonymousAuthentication.SetAttributeValue("enabled", settings.EnableAnonymousAuthentication);

                Log.Information("Anonymous Authentication enabled: {0}", settings.EnableAnonymousAuthentication);

                // Basic Authentication
                var basicAuthentication = authentication.GetChildElement("basicAuthentication");

                basicAuthentication.SetAttributeValue("enabled", settings.EnableBasicAuthentication);
                basicAuthentication.SetAttributeValue("userName", settings.Username);
                basicAuthentication.SetAttributeValue("password", settings.Password);

                Log.Information("Basic Authentication enabled: {0}", settings.EnableBasicAuthentication);

                // Windows Authentication
                var windowsAuthentication = authentication.GetChildElement("windowsAuthentication");

                windowsAuthentication.SetAttributeValue("enabled", settings.EnableWindowsAuthentication);

                Log.Information("Windows Authentication enabled: {0}", settings.EnableWindowsAuthentication);
            }
        }

        /// <summary>
        /// Sets the authorization settings for the site
        /// </summary>
        /// <param name="server">The atype of server</param>
        /// <param name="site">The name of the site</param>
        /// <param name="appPath">The application path</param>
        /// <param name="settings">The authorization settings</param>
        protected void SetAuthorization(string server, string site, string appPath, AuthorizationSettings settings)
        {
            if (settings != null)
            {
                //Authorization
                var config = Server.GetApplicationHostConfiguration();

                var locationPath = site + appPath;
                var authorization = config.GetSection("system." + server + "/security/authorization", locationPath);
                var authCollection = authorization.GetCollection();

                var addElement = authCollection.CreateElement("add");
                addElement.SetAttributeValue("accessType", "Allow");

                switch (settings.AuthorizationType)
                {
                    case AuthorizationType.AllUsers:
                        addElement.SetAttributeValue("users", "*");
                        Log.Information("Authorization for all users.");
                        break;

                    case AuthorizationType.SpecifiedUser:
                        addElement.SetAttributeValue("users", string.Join(", ", settings.Users));
                        Log.Information("Authorization resticted to specific users {0}.", settings.Users);
                        break;

                    case AuthorizationType.SpecifiedRoleOrUserGroup:
                        addElement.SetAttributeValue("roles", string.Join(", ", settings.Roles));
                        Log.Information("Authorization resticted to specific roles {0}.", settings.Roles);
                        break;
                }

                //Permissions
                var permissions = new List<string>();

                if (settings.CanRead)
                {
                    permissions.Add("Read");
                }
                if (settings.CanWrite)
                {
                    permissions.Add("Write");
                }

                addElement.SetAttributeValue("permissions", string.Join(", ", permissions));

                authCollection.Clear();
                authCollection.Add(addElement);
            }
        }

        /// <summary>
        /// Delets a site from IIS
        /// </summary>
        /// <param name="name">The name of the site to delete</param>
        /// <returns>If the site was deleted.</returns>
        public bool Delete(string name)
        {
            var site = Server.Sites.FirstOrDefault(p => p.Name == name);
            if (site == null)
            {
                Log.Information("Site '{0}' not found.", name);
                return true;
            }
            Server.Sites.Remove(site);
            Server.CommitChanges();
            Log.Information("Site '{0}' deleted.", site.Name);
            return false;
        }

        /// <summary>
        /// Checks if a site exists in IIS
        /// </summary>
        /// <param name="name">The name of the site to check</param>
        /// <returns>If the site exists.</returns>
        public bool Exists(string name)
        {
            if (Server.Sites.SingleOrDefault(p => p.Name == name) != null)
            {
                Log.Information("The site '{0}' exists.", name);
                return true;
            }
            Log.Information("The site '{0}' does not exist.", name);
            return false;
        }

        /// <summary>
        /// Starts a IIS site
        /// </summary>
        /// <param name="name">The name of the site to start</param>
        /// <returns>If the site was started.</returns>
        public bool Start(string name)
        {
            var site = Server.Sites.FirstOrDefault(p => p.Name == name);
            if (site == null)
            {
                Log.Information("Site '{0}' not found.", name);
                return false;
            }
            try
            {
                site.Start();
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                Log.Information("Waiting for IIS to activate new config");
                Thread.Sleep(1000);
            }
            Log.Information("Site '{0}' started.", site.Name);
            return true;
        }

        /// <summary>
        /// Stops a IIS site
        /// </summary>
        /// <param name="name">The name of the site to stop</param>
        /// <returns>If the site was stopped.</returns>
        public bool Stop(string name)
        {
            var site = Server.Sites.FirstOrDefault(p => p.Name == name);
            if (site == null)
            {
                Log.Information("Site '{0}' not found.", name);
                return false;
            }
            try
            {
                site.Stop();
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                Log.Information("Waiting for IIS to activate new config");
                Thread.Sleep(1000);
            }
            Log.Information("Site '{0}' stopped.", site.Name);
            return true;
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
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(siteName))
            {
                throw new ArgumentException("Site name cannot be null!");
            }

            //Get Site
            Site site = Server.Sites.SingleOrDefault(p => p.Name == siteName);

            if (site != null)
            {
                if (site.Bindings.FirstOrDefault(b => b.Protocol == settings.BindingProtocol.ToString() && b.BindingInformation == settings.BindingInformation) != null)
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

                site.Bindings.Add(newBinding);
                Server.CommitChanges();

                Log.Information("Binding added.");
                return true;
            }
            throw new Exception("Site: " + siteName + " does not exist.");
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
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(siteName))
            {
                throw new ArgumentException("Site name cannot be null!");
            }

            //Get Site
            Site site = Server.Sites.SingleOrDefault(p => p.Name == siteName);

            if (site != null)
            {
                Binding binding = site.Bindings.FirstOrDefault(b => b.Protocol == settings.BindingProtocol.ToString() && b.BindingInformation == settings.BindingInformation);

                if (binding != null)
                {
                    //Remove Binding
                    site.Bindings.Remove(binding);
                    Server.CommitChanges();

                    Log.Information("Binding removed.");
                    return true;
                }
                Log.Information("A binding with the same ip, port and host header does not exists.");
                return false;
            }
            throw new Exception("Site: " + siteName + " does not exist.");
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
                throw new ArgumentNullException(nameof(settings));
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
            ApplicationPool appPool = Server.ApplicationPools.SingleOrDefault(p => p.Name == settings.ApplicationPool);

            if (appPool == null)
            {
                throw new Exception("Application Pool '" + settings.ApplicationPool + "' does not exist.");
            }

            //Get Site
            Site site = Server.Sites.SingleOrDefault(p => p.Name == settings.SiteName);

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
                vDir.Path = settings.VirtualDirectory;
                vDir.PhysicalPath = GetPhysicalDirectory(settings);

                app.VirtualDirectories.Add(vDir);

                SetAuthentication("webServer", settings.SiteName, settings.ApplicationPath, settings.Authentication);
                SetAuthorization("webServer", settings.SiteName, settings.ApplicationPath, settings.Authorization);

                site.Applications.Add(app);
                Server.CommitChanges();

                return true;
            }
            throw new Exception("Site '" + settings.SiteName + "' does not exist.");
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
                throw new ArgumentNullException(nameof(settings));
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
            Site site = Server.Sites.SingleOrDefault(p => p.Name == settings.SiteName);

            if (site != null)
            {
                //Get Application
                Application app = site.Applications.SingleOrDefault(p => p.Path == settings.ApplicationPath);
                if (app == null)
                {
                    throw new Exception("Application '" + settings.ApplicationPath + "' does not exists.");
                }
                site.Applications.Remove(app);
                Server.CommitChanges();
                return true;
            }
            throw new Exception("Site '" + settings.SiteName + "' does not exist.");
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
                throw new ArgumentNullException(nameof(settings));
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
            Site site = Server.Sites.SingleOrDefault(p => p.Name == settings.SiteName);

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
            vDir.PhysicalPath = GetPhysicalDirectory(settings);

            app.VirtualDirectories.Add(vDir);

            //this.SetAuthentication("webServer", settings.SiteName, settings.ApplicationPath, settings.Authentication);
            //this.SetAuthorization("webServer", settings.SiteName, settings.ApplicationPath, settings.Authorization);

            Server.CommitChanges();

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
                throw new ArgumentNullException(nameof(settings));
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
            Site site = Server.Sites.SingleOrDefault(p => p.Name == settings.SiteName);

            if (site != null)
            {
                //Get Application
                Application app = site.Applications.SingleOrDefault(p => p.Path == settings.ApplicationPath);

                if (app == null)
                {
                    throw new Exception("Application '" + settings.ApplicationPath + "' does not exist.");
                }
                VirtualDirectory vd = app.VirtualDirectories.FirstOrDefault(p => p.Path == settings.Path);
                if (vd == null)
                {
                    throw new Exception("Virtual directory '" + settings.Path + "' does not exist.");
                }
                app.VirtualDirectories.Remove(vd);
                Server.CommitChanges();

                return true;
            }
            throw new Exception("Site '" + settings.SiteName + "' does not exist.");
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
                throw new ArgumentNullException(nameof(settings));
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
            Site site = Server.Sites.SingleOrDefault(p => p.Name == settings.SiteName);

            if (site != null)
            {
                //Get Application
                Application app = site.Applications.SingleOrDefault(p => p.Path == settings.ApplicationPath);
                if (app == null)
                {
                    throw new Exception("Application '" + settings.ApplicationPath + "' does not exist.");
                }
                VirtualDirectory vd = app.VirtualDirectories.FirstOrDefault(p => p.Path == settings.Path);
                return vd != null;
            }
            throw new Exception("Site '" + settings.SiteName + "' does not exist.");
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
                throw new ArgumentNullException(nameof(settings));
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
            Site site = Server.Sites.SingleOrDefault(p => p.Name == settings.SiteName);

            if (site != null)
            {
                //Get Application
                return site.Applications.Any(p => p.Path == settings.ApplicationPath);
            }
            throw new Exception("Site '" + settings.SiteName + "' does not exist.");
        }
    }
}