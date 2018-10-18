#region Using Statements
using System;

using Cake.Core.IO;
#endregion



namespace Cake.IIS
{
    public class ApplicationSettings : IDirectorySettings
    {
        #region Constructors
        public ApplicationSettings()
        {

        }
        #endregion





        #region Properties
        public string ComputerName { get; set; }



        public string SiteName { get; set; }

        public string ApplicationPath { get; set; }



        public string ApplicationPool { get; set; }

        public DirectoryPath WorkingDirectory { get; set; }

        public DirectoryPath PhysicalDirectory { get; set; }



        public AuthenticationSettings Authentication { get; set; }

        public AuthorizationSettings Authorization { get; set; }

        public string AlternateEnabledProtocols { get; set; }



        /// <summary>
        /// Flag to get or set of directory browsing should be enabled.
        /// </summary>
        public bool EnableDirectoryBrowsing { get; set; }



        [Obsolete("Use Authentication.UserName")]
        public string UserName
        {
            get
            {
                return this.Authentication.Username;
            }
            set
            {
                if (this.Authentication == null)
                {
                    this.Authentication = new AuthenticationSettings();
                }

                this.Authentication.Username = value;
            }
        }

        [Obsolete("Use Authentication.Password")]
        public string Password
        {
            get
            {
                return this.Authentication.Password;
            }
            set
            {
                if (this.Authentication == null)
                {
                    this.Authentication = new AuthenticationSettings();
                }

                this.Authentication.Password = value;
            }
        }
        #endregion
    }
}