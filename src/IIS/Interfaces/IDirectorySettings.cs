using Cake.Core.IO;

namespace Cake.IIS
{
    /// <summary>
    ///  Common interface for location based settings
    /// </summary>
    public interface IDirectorySettings
    {
        /// <summary>
        /// Gets or sets optional computer name to manage IIS on
        /// </summary>
        string ComputerName { get; set; }

        /// <summary>
        /// Gets or sets the cake working directory
        /// </summary>
        DirectoryPath WorkingDirectory { get; set; }

        /// <summary>
        /// Gets or sets the physical IIS directory
        /// </summary>
        DirectoryPath PhysicalDirectory { get; set; }
    }
}