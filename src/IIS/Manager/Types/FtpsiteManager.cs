#region Using Statements
using Microsoft.Web.Administration;

using Cake.Core;
using Cake.Core.Diagnostics;
#endregion



namespace Cake.IIS
{
    /// <summary>
    /// Class for managing FTP sites
    /// </summary>
    public class FtpsiteManager : BaseSiteManager
    {
        #region Constructors (1)
        /// <summary>
        /// Initializes a new instance of the <see cref="FtpsiteManager" /> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="log">The log.</param>
        public FtpsiteManager(ICakeEnvironment environment, ICakeLog log)
            : base(environment, log)
        {

        }
        #endregion





        #region Methods (2)
        /// <summary>
        /// Creates a new instance of the <see cref="FtpsiteManager" /> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="log">The log.</param>
        /// <param name="server">The <see cref="ServerManager" /> to connect to.</param>
        /// <returns>a new instance of the <see cref="FtpsiteManager" />.</returns>
        public static FtpsiteManager Using(ICakeEnvironment environment, ICakeLog log, ServerManager server)
        {
            FtpsiteManager manager = new FtpsiteManager(environment, log);

            manager.SetServer(server);

            return manager;
        }



        /// <summary>
        /// Creates an FTP site
        /// </summary>
        /// <param name="settings">The settings of the FTP site to add</param>
        /// <returns>If the FTP site was added.</returns>
        public void Create(FtpsiteSettings settings)
        {
            //Create Site
            bool exists;
            Site site = base.CreateSite(settings, out exists);



            if (!exists)
            {
                // SSL policy
                var ssl = site
                    .GetChildElement("ftpServer")
                    .GetChildElement("security")
                    .GetChildElement("ssl");

                ssl.SetAttributeValue("controlChannelPolicy", "SslAllow");
                ssl.SetAttributeValue("dataChannelPolicy", "SslAllow");



                // Host name support
                    var hostNameSupport = ServerManager
                    .GetApplicationHostConfiguration()
                    .GetSection("system.ftpServer/serverRuntime")
                    .GetChildElement("hostNameSupport");

                hostNameSupport.SetAttributeValue("useDomainNameAsHostName", true);

                    ServerManager.CommitChanges();

                _Log.Information("Ftp Site '{0}' created.", settings.Name);
            }
        }
        #endregion
    }
}