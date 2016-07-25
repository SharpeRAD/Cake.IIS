namespace Cake.IIS.Manager.Types
{
    /// <summary>
    /// Hold settings for virtual application creation
    /// </summary>
    public class VirtualApplicationSettings
    {
        /// <summary>
        /// Name of the virtual applicaiton
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Path where virtual application is location on the disk or network share
        /// </summary>
        public string PhysicalPath { get; set; }
        /// <summary>
        /// Name of website that is a parent for the application
        /// </summary>
        public string ParentWebSite { get; set; }
        /// <summary>
        /// Name of the application pool.
        /// </summary>
        public string ApplicationPoolName { get; set; }
        /// <summary>
        /// Enabled additional protocols such as net.tcp
        /// </summary>
        public string EnabledProtocols { get; set; }
        /// <summary>
        /// Defines whether to override current cofiguration
        /// </summary>
        public bool Overwrite { get; set; }
        
    }
}