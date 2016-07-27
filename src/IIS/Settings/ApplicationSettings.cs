#region Using Statements

using System;
using Cake.Core.IO;
#endregion



namespace Cake.IIS
{
    public class ApplicationSettings : IDirectorySettings
    {
        private AuthenticationSettings _authentication;

        #region Constructor (1)
        public ApplicationSettings()
        {
            Authentication = new AuthenticationSettings();
            
        }
        #endregion





        #region Properties (9)
        public string ComputerName { get; set; }



        public string SiteName { get; set; }

        public string ApplicationPath { get; set; }


        public string ApplicationPool { get; set; }



        public string VirtualDirectory { get; set; }

        public DirectoryPath WorkingDirectory { get; set; }

        public DirectoryPath PhysicalDirectory { get; set; }


        public AuthenticationSettings Authentication
        {
            get { return _authentication; }
            set { if(value == null) throw new ArgumentException("Authentication"); _authentication = value; }
        }

        [Obsolete("Use Authentication.UserName")]
        public string UserName {  get { return Authentication.Username; } set { Authentication.Username = value; } }

        [Obsolete("Use Authentication.Password")]
        public string Password { get { return Authentication.Password; } set { Authentication.Password = value; } }


        public bool Overwrite { get; set; }
        #endregion
    }
}