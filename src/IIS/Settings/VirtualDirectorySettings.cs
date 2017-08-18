using Cake.Core.IO;

namespace Cake.IIS
{
    public class VirtualDirectorySettings : IDirectorySettings
    {
        public VirtualDirectorySettings()
        {
        }

        public string ComputerName { get; set; }

        public DirectoryPath PhysicalDirectory { get; set; }

        public DirectoryPath WorkingDirectory { get; set; }

        public string SiteName { get; set; }

        public string ApplicationPath { get; set; }

        public string Path { get; set; }

        public AuthenticationSettings Authentication { get; set; }

        public AuthorizationSettings Authorization { get; set; }
    }
}