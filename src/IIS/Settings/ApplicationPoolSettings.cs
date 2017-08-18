using System;

namespace Cake.IIS
{
    public class ApplicationPoolSettings
    {
        private string _username;

        public ApplicationPoolSettings()
        {
            Name = "ASP.NET v4.0";
            ManagedRuntimeVersion = "v4.0";

            IdentityType = IdentityType.ApplicationPoolIdentity;
            ClassicManagedPipelineMode = false;
            Enable32BitAppOnWin64 = false;

            Autostart = true;
            Overwrite = false;

            PingInterval = TimeSpan.MinValue;
            PingResponseTime = TimeSpan.MinValue;
            IdleTimeout = TimeSpan.MinValue;
            ShutdownTimeLimit = TimeSpan.MinValue;
            StartupTimeLimit = TimeSpan.MinValue;
        }

        public string Name { get; set; }

        public IdentityType IdentityType { get; set; }

        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;

                if (!string.IsNullOrEmpty(value))
                {
                    IdentityType = IdentityType.SpecificUser;
                }
            }
        }

        public string Password { get; set; }

        public string ManagedRuntimeVersion { get; set; }

        public bool ClassicManagedPipelineMode { get; set; }

        public bool Enable32BitAppOnWin64 { get; set; }

        public bool LoadUserProfile { get; set; }

        public long MaxProcesses { get; set; }

        public bool PingingEnabled { get; set; }

        public TimeSpan PingInterval { get; set; }

        public TimeSpan PingResponseTime { get; set; }

        public TimeSpan IdleTimeout { get; set; }

        public TimeSpan ShutdownTimeLimit { get; set; }

        public TimeSpan StartupTimeLimit { get; set; }

        public bool Autostart { get; set; }

        public bool Overwrite { get; set; }
    }
}