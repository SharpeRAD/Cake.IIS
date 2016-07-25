#region Using Statements
    using System;
    using System.Linq;
    using System.IO;
    using System.Threading;

    using Cake.Core;
    using Cake.IIS.Manager.Types;
    using Microsoft.Web.Administration;

    using NSubstitute;
#endregion



namespace Cake.IIS.Tests.Utils
{
    internal static class CakeHelper
    {
        #region Functions (4)
            //Cake
            public static ICakeEnvironment CreateEnvironment()
            {
                var environment = Substitute.For<ICakeEnvironment>();
                environment.WorkingDirectory = Directory.GetCurrentDirectory();

                return environment;
            }



            //Managers
            public static ApplicationPoolManager CreateApplicationPoolManager()
            {
                ApplicationPoolManager manager = new ApplicationPoolManager(CakeHelper.CreateEnvironment(), new DebugLog());

                manager.SetServer();

                return manager;
            }

            public static FtpsiteManager CreateFtpsiteManager()
            {
                FtpsiteManager manager = new FtpsiteManager(CakeHelper.CreateEnvironment(), new DebugLog());

                manager.SetServer();

                return manager;
            }

            public static WebsiteManager CreateWebsiteManager()
            {
                WebsiteManager manager = new WebsiteManager(CakeHelper.CreateEnvironment(), new DebugLog());

                manager.SetServer();

                return manager;
            }
        
            public static WebFarmManager CreateWebFarmManager()
            {
                WebFarmManager manager = new WebFarmManager(CakeHelper.CreateEnvironment(), new DebugLog());

                manager.SetServer();

                return manager;
            }

        public static VirtualApplicationSettings GetVirtualAppSettings()
        {
            var websiteSettings = GetWebsiteSettings();
            return new VirtualApplicationSettings()
            {
                ApplicationPoolName = GetAppPoolSettings().Name,
                Name = "Blog",
                ParentWebSite = websiteSettings.Name,
                PhysicalPath = Directory.GetCurrentDirectory(),
                Authentication = ApplicationAuthentication.Windows
            };
        }

        //Settings
            public static ApplicationPoolSettings GetAppPoolSettings()
            {
                return new ApplicationPoolSettings
                {
                    Name = "Superman",
                    IdentityType = IdentityType.NetworkService,
                    Autostart = true,
                    MaxProcesses = 1,
                    Enable32BitAppOnWin64 = false,

                    IdleTimeout = TimeSpan.FromMinutes(20),
                    ShutdownTimeLimit = TimeSpan.FromSeconds(90),
                    StartupTimeLimit = TimeSpan.FromSeconds(90),

                    PingingEnabled = true,
                    PingInterval = TimeSpan.FromSeconds(30),
                    PingResponseTime = TimeSpan.FromSeconds(90),
                    Overwrite = false
                };
            }

            public static WebsiteSettings GetWebsiteSettings()
            {
                return new WebsiteSettings
                {
                    Name = "Superman",
                    BindingProtocol = BindingProtocol.Http,
                    HostName = "superman.web",
                    PhysicalDirectory = "./Test/",
                    ApplicationPool = CakeHelper.GetAppPoolSettings(),
                    Port = 80,
                    ServerAutoStart = true,
                    Overwrite = false
                };
            }

            public static WebFarmSettings GetWebFarmSettings()
            {
                return new WebFarmSettings
                {
                    Name = "Batman",
                    Servers = new string[] { "Gotham", "Metroplis" }
                };
            }



            //Website
            public static void CreateWebsite(WebsiteSettings settings)
            {
                WebsiteManager manager = CakeHelper.CreateWebsiteManager();

                manager.Create(settings);
            }

            public static void DeleteWebsite(string name)
            {
                using (var server = new ServerManager())
                {
                    var site = server.Sites.FirstOrDefault(x => x.Name == name);

                    if (site != null)
                    {
                        server.Sites.Remove(site);
                        server.CommitChanges();
                    }
                }
            }

            public static Site GetWebsite(string name)
            {
                using (var serverManager = new ServerManager())
                {
                    return serverManager.Sites.FirstOrDefault(x => x.Name == name);
                }
            }

            public static void StartWebsite(string name)
            {
                using (var server = new ServerManager())
                {
                    Site site = server.Sites.FirstOrDefault(x => x.Name == name);

                    if (site != null)
                    {
                        try
                        {
                            site.Start();
                        }
                        catch (System.Runtime.InteropServices.COMException)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }
            }

            public static void StopWebsite(string name)
            {
                using (var server = new ServerManager())
                {
                    Site site = server.Sites.FirstOrDefault(x => x.Name == name);

                    if (site != null)
                    {
                        try
                        {
                            site.Stop();
                        }
                        catch (System.Runtime.InteropServices.COMException)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }
            }



            //Pool
            public static void CreatePool(ApplicationPoolSettings settings)
            {
                ApplicationPoolManager manager = CakeHelper.CreateApplicationPoolManager();

                manager.Create(settings);
            }

            public static void DeletePool(string name)
            {
                using (var server = new ServerManager())
                {
                    ApplicationPool pool = server.ApplicationPools.FirstOrDefault(x => x.Name == name);

                    if (pool != null)
                    {
                        server.ApplicationPools.Remove(pool);
                        server.CommitChanges();
                    }
                }
            }

            public static ApplicationPool GetPool(string name)
            {
                using (var server = new ServerManager())
                {
                    return server.ApplicationPools.FirstOrDefault(x => x.Name == name);
                }
            }

            public static void StartPool(string name)
            {
                using (var server = new ServerManager())
                {
                    ApplicationPool pool = server.ApplicationPools.FirstOrDefault(x => x.Name == name);

                    if (pool != null)
                    {
                        try
                        {
                            pool.Start();
                        }
                        catch (System.Runtime.InteropServices.COMException)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }
            }

            public static void StopPool(string name)
            {
                using (var server = new ServerManager())
                {
                    ApplicationPool pool = server.ApplicationPools.FirstOrDefault(x => x.Name == name);

                    if (pool != null)
                    {
                        try
                        {
                            pool.Stop();
                        }
                        catch (System.Runtime.InteropServices.COMException)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }
            }



            //WebFarm
            public static void CreateWebFarm(WebFarmSettings settings)
            {
                WebFarmManager manager = CakeHelper.CreateWebFarmManager();

                manager.Create(settings);
            }

            public static void DeleteWebFarm(string name)
            {
                using (var serverManager = new ServerManager())
                {
                    Configuration config = serverManager.GetApplicationHostConfiguration();

                    ConfigurationSection section = config.GetSection("webFarms");
                    ConfigurationElementCollection farms = section.GetCollection();

                    ConfigurationElement farm = farms.FirstOrDefault(f => f.GetAttributeValue("name").ToString() == name);

                    if (farm != null)
                    {
                        farms.Remove(farm);
                        serverManager.CommitChanges();
                    }
                }
            }

            public static ConfigurationElement GetWebFarm(string name)
            {
                using (var serverManager = new ServerManager())
                {
                    Configuration config = serverManager.GetApplicationHostConfiguration();

                    ConfigurationSection section = config.GetSection("webFarms");
                    ConfigurationElementCollection farms = section.GetCollection();

                    return farms.FirstOrDefault(f => f.GetAttributeValue("name").ToString() == name);
                }
            }

        #endregion

        public static void CreateVirtualApplication(VirtualApplicationSettings settings)
        {
            var manager = CakeHelper.CreateVirtualApplicationManager();

            manager.Create(settings);
        }

        private static VirtualApplicationManager CreateVirtualApplicationManager()
        {
            var manager = new VirtualApplicationManager(CakeHelper.CreateEnvironment(), new DebugLog());
            manager.SetServer();
            return manager;
        }

        public static Application GetVirtualApplication(string webSite, string appName)
        {
            if (!appName.StartsWith("/"))
                appName = "/" + appName;
            using (var server = new ServerManager())
            {
                var site = server.Sites.FirstOrDefault(x => x.Name == webSite);
                return site?.Applications.FirstOrDefault(x => x.Path == appName);
            }
        }

        public static void DeleteApplication(string parentWebSite, string name)
        {
            var manager = CakeHelper.CreateVirtualApplicationManager();

            manager.Delete(parentWebSite, name);
        }
    }
}
