using System;
using Cake.Core.IO;

namespace Cake.IIS
{
    public class ApplicationSettings : IDirectorySettings
    {
        public ApplicationSettings()
        {
        }

        public string ComputerName { get; set; }

        public string SiteName { get; set; }

        public string ApplicationPath { get; set; }

        public string ApplicationPool { get; set; }

        public string VirtualDirectory { get; set; }

        public DirectoryPath WorkingDirectory { get; set; }

        public DirectoryPath PhysicalDirectory { get; set; }

        public AuthenticationSettings Authentication { get; set; }

        public AuthorizationSettings Authorization { get; set; }

        public string AlternateEnabledProtocols { get; set; }

        [Obsolete("Use Authentication.UserName")]
        public string UserName
        {
            get
            {
                return Authentication.Username;
            }
            set
            {
                if (Authentication == null)
                {
                    Authentication = new AuthenticationSettings();
                }
                Authentication.Username = value;
            }
        }

        [Obsolete("Use Authentication.Password")]
        public string Password
        {
            get
            {
                return Authentication.Password;
            }
            set
            {
                if (Authentication == null)
                {
                    Authentication = new AuthenticationSettings();
                }
                Authentication.Password = value;
            }
        }
    }
}