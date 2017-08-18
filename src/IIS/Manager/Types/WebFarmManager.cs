using System;
using System.Linq;
using Microsoft.Web.Administration;
using Cake.Core;
using Cake.Core.Diagnostics;

namespace Cake.IIS
{
    /// <summary>
    /// Class for managing IIS WebFarms
    /// </summary>
    public class WebFarmManager : BaseManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebFarmManager" /> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="log">The log.</param>
        public WebFarmManager(ICakeEnvironment environment, ICakeLog log)
            : base(environment, log)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WebFarmManager" /> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="log">The log.</param>
        /// <param name="server">The <see cref="ServerManager" /> to connect to.</param>
        /// <returns>a new instance of the <see cref="WebFarmManager" />.</returns>
        public static WebFarmManager Using(ICakeEnvironment environment, ICakeLog log, ServerManager server)
        {
            WebFarmManager manager = new WebFarmManager(environment, log);
            manager.SetServer(server);
            return manager;
        }

        /// <summary>
        /// Creates a WebFarm
        /// </summary>
        /// <param name="settings">The settings of the WebFarm</param>
        /// <returns>If the WebFarm was added.</returns>
        public void Create(WebFarmSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(settings.Name))
            {
                throw new ArgumentException("WebFarm name cannot be null!");
            }

            //Get Farm
            ConfigurationElementCollection farms = GetFarms();
            ConfigurationElement farm = farms.FirstOrDefault(f => f.GetAttributeValue("name").ToString() == settings.Name);

            if (farm != null)
            {
                Log.Information("WebFarm '{0}' already exists.", settings.Name);

                if (settings.Overwrite)
                {
                    Log.Information("WebFarm '{0}' will be overriden by request.", settings.Name);
                    Delete(settings.Name);
                }
                else
                {
                    return;
                }
            }

            //Create Farm
            farm = farms.CreateElement("webFarm");
            farm["name"] = settings.Name;

            //Add Server
            ConfigurationElementCollection servers = farm.GetCollection();

            foreach (string server in settings.Servers)
            {
                ConfigurationElement serverElement = servers.CreateElement("server");
                serverElement["address"] = server;
                servers.Add(serverElement);
                Log.Information("Adding server '{0}'.", server);
            }

            farms.Add(farm);
            Server.CommitChanges();

            Log.Information("WebFarm created.");
        }

        /// <summary>
        /// Delete an WebFarm
        /// </summary>
        /// <param name="name">The name of the WebFarm</param>
        /// <returns>If the WebFarm was deleted.</returns>
        public bool Delete(string name)
        {
            ConfigurationElementCollection farms = GetFarms();
            ConfigurationElement farm = farms.FirstOrDefault(f => f.GetAttributeValue("name").ToString() == name);

            if (farm != null)
            {
                farms.Remove(farm);
                Server.CommitChanges();
                Log.Information("WebFarm deleted.");
                return true;
            }
            Log.Information("The webfarm '{0}' does not exists.", name);
            return false;
        }

        /// <summary>
        /// Checks if a WebFarm exists
        /// </summary>
        /// <param name="name">The name of the WebFarm</param>
        /// <returns>If the WebFarm exists.</returns>
        public bool Exists(string name)
        {
            ConfigurationElementCollection farms = GetFarms();
            ConfigurationElement farm = farms.FirstOrDefault(f => f.GetAttributeValue("name").ToString() == name);

            if (farm != null)
            {
                Log.Information("The webfarm '{0}' exists.", name);
                return true;
            }
            Log.Information("The webfarm '{0}' does not exists.", name);
            return false;
        }

        /// <summary>
        /// Adds a server to the WebFarm
        /// </summary>
        /// <param name="farm">The name of the WebFarm</param>
        /// <param name="address">The address of the server</param>
        /// <returns>If the server was added.</returns>
        public bool AddServer(string farm, string address)
        {
            ConfigurationElement farmElement = GetFarm(farm);

            if (farmElement != null)
            {
                ConfigurationElementCollection servers = farmElement.GetCollection();
                ConfigurationElement server = servers.FirstOrDefault(f => f.GetAttributeValue("address").ToString() == address);

                if (server == null)
                {
                    ConfigurationElement serverElement = servers.CreateElement("server");
                    serverElement["address"] = server;

                    servers.Add(serverElement);
                    Server.CommitChanges();

                    Log.Information("Adding server '{0}'.", address);
                    return true;
                }
                Log.Information("The server '{0}' already exists.", address);
                return false;
            }
            return false;
        }

        /// <summary>
        /// Removes a server to the WebFarm
        /// </summary>
        /// <param name="farm">The name of the WebFarm</param>
        /// <param name="address">The address of the server</param>
        /// <returns>If the server was removed.</returns>
        public bool RemoveServer(string farm, string address)
        {
            ConfigurationElement farmElement = GetFarm(farm);
            if (farmElement != null)
            {
                ConfigurationElementCollection servers = farmElement.GetCollection();
                ConfigurationElement server = servers.FirstOrDefault(f => f.GetAttributeValue("address").ToString() == address);

                if (server != null)
                {
                    servers.Remove(server);
                    Server.CommitChanges();

                    Log.Information("Removed server '{0}'.", address);
                    return true;
                }
                Log.Information("The server '{0}' does not exists.", address);
                return false;
            }
            return false;
        }

        /// <summary>
        /// Checks if a server belongs to the WebFarm
        /// </summary>
        /// <param name="farm">The name of the WebFarm</param>
        /// <param name="address">The address of the server</param>
        /// <returns>If the server belongs to the WebFarm.</returns>
        public bool ServerExists(string farm, string address)
        {
            ConfigurationElement farmElement = GetFarm(farm);

            if (farmElement != null)
            {
                ConfigurationElementCollection servers = farmElement.GetCollection();
                ConfigurationElement server = servers.FirstOrDefault(f => f.GetAttributeValue("address").ToString() == address);

                if (server != null)
                {
                    Log.Information("The server '{0}' exists.", address);
                    return true;
                }
                Log.Information("The server '{0}' does not exists.", address);
                return false;
            }
            return false;
        }

        /// <summary>
        /// Marks a server as healthy
        /// </summary>
        /// <param name="farm">The name of the WebFarm</param>
        /// <param name="address">The address of the server</param>
        public void SetServerHealthy(string farm, string address)
        {
            ConfigurationElement arrElement = GetServerArr(farm, address);

            if (arrElement != null)
            {
                ConfigurationMethod method = arrElement.Methods["SetHealthy"];
                ConfigurationMethodInstance instance = method.CreateInstance();

                instance.Execute();

                Log.Information("Marking the server '{0}' as healthy.", address);
            }
        }

        /// <summary>
        /// Marks a server as unhealthy
        /// </summary>
        /// <param name="farm">The name of the WebFarm</param>
        /// <param name="address">The address of the server</param>
        public void SetServerUnhealthy(string farm, string address)
        {
            ConfigurationElement arrElement = GetServerArr(farm, address);

            if (arrElement != null)
            {
                ConfigurationMethod method = arrElement.Methods["SetUnhealthy"];
                ConfigurationMethodInstance instance = method.CreateInstance();

                instance.Execute();

                Log.Information("Marking the server '{0}' as unhealthy.", address);
            }
        }

        /// <summary>
        /// Marks a server as available
        /// </summary>
        /// <param name="farm">The name of the WebFarm</param>
        /// <param name="address">The address of the server</param>
        public void SetServerAvailable(string farm, string address)
        {
            ConfigurationElement arrElement = GetServerArr(farm, address);

            if (arrElement != null)
            {
                ConfigurationMethod method = arrElement.Methods["SetState"];
                ConfigurationMethodInstance instance = method.CreateInstance();

                instance.Input.Attributes[0].Value = 0;
                instance.Execute();

                Log.Information("Marking the server '{0}' as available.", address);
            }
        }

        /// <summary>
        /// Marks a server as unavailable
        /// </summary>
        /// <param name="farm">The name of the WebFarm</param>
        /// <param name="address">The address of the server</param>
        public void SetServerUnavailable(string farm, string address)
        {
            ConfigurationElement arrElement = GetServerArr(farm, address);

            if (arrElement != null)
            {
                ConfigurationMethod method = arrElement.Methods["SetState"];
                ConfigurationMethodInstance instance = method.CreateInstance();

                instance.Input.Attributes[0].Value = 3;
                instance.Execute();

                Log.Information("Marking the server '{0}' as unavailable.", address);
            }
        }

        /// <summary>
        /// Checks if a server is healthy
        /// </summary>
        /// <param name="farm">The name of the WebFarm</param>
        /// <param name="address">The address of the server</param>
        /// <returns>If the server is healthy.</returns>
        public bool GetServerIsHealthy(string farm, string address)
        {
            ConfigurationElement arrElement = GetServerArr(farm, address);

            if (arrElement != null)
            {
                ConfigurationElement countersElement = arrElement.GetChildElement("counters");

                string value = countersElement.GetAttributeValue("isHealthy").ToString().ToLower();

                return value == "true";
            }
            return false;
        }

        /// <summary>
        /// Gets the state of a server in a WebFarm
        /// </summary>
        /// <param name="farm">The name of the WebFarm</param>
        /// <param name="address">The address of the server</param>
        /// <returns>Gets the state of the server.</returns>
        public string GetServerState(string farm, string address)
        {
            ConfigurationElement arrElement = GetServerArr(farm, address);

            if (arrElement != null)
            {
                ConfigurationElement countersElement = arrElement.GetChildElement("counters");

                return countersElement.GetAttributeValue("isHealthy").ToString();
            }
            return "";
        }

        //Helpers
        private ConfigurationElementCollection GetFarms()
        {
            Configuration config = Server.GetApplicationHostConfiguration();
            ConfigurationSection section = config.GetSection("webFarms");
            return section.GetCollection();
        }

        private ConfigurationElement GetFarm(string name)
        {
            ConfigurationElementCollection farms = GetFarms();
            ConfigurationElement farm = farms.FirstOrDefault(f => f.GetAttributeValue("name").ToString() == name);

            if (farm == null)
            {
                Log.Information("The webfarm '{0}' does not exists.", name);
            }

            return farm;
        }

        private ConfigurationElement GetServer(ConfigurationElement farm, string address)
        {
            ConfigurationElementCollection servers = farm.GetCollection();
            ConfigurationElement server = servers.FirstOrDefault(f => f.GetAttributeValue("address").ToString() == address);

            if (server == null)
            {
                Log.Information("The server '{0}' does not exists.", address);
            }

            return server;
        }

        private ConfigurationElement GetServerArr(string farm, string address)
        {
            ConfigurationElement farmElement = GetFarm(farm);

            if (farmElement != null)
            {
                ConfigurationElement serverElement = GetServer(farmElement, address);

                if (serverElement != null)
                {
                    return serverElement.GetChildElement("applicationRequestRouting");
                }
            }

            return null;
        }
    }
}