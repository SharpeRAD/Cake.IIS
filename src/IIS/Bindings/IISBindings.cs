using Cake.IIS.Bindings;

namespace Cake.IIS
{
    /// <summary>
    /// Helper class to expose common bindings
    /// </summary>
    public static class IISBindings
    {
        /// <summary>
        /// Creates http binding (port: 80)
        /// </summary>
        public static BindingSettings Http => new BindingSettings(BindingProtocol.Http)
        {
            HostName = "*",
            IpAddress = "*",
            Port = 80
        };

        /// <summary>
        /// Creates https binding (port: 443)
        /// </summary>
        public static BindingSettings Https => new BindingSettings(BindingProtocol.Https)
        {
            HostName = "*",
            IpAddress = "*",
            Port = 443
        };

        /// <summary>
        /// Creates ftp binding (port: 21)
        /// </summary>
        public static BindingSettings Ftp => new PortBindingSettings(BindingProtocol.Ftp)
        {
            Port = 21
        };

        /// <summary>
        /// Creates net.tcp binding (port: 808)
        /// </summary>
        public static BindingSettings NetTcp => new PortBindingSettings(BindingProtocol.NetTcp)
        {
            Port = 808,
            HostName = "*"
        };

        /// <summary>
        /// Creates net.pipe binding
        /// </summary>
        public static BindingSettings NetPipe => new HostBindingSettings(BindingProtocol.NetPipe)
        {
            HostName = "*"
        };

        /// <summary>
        /// Creates net.msmq binding.
        /// </summary>
        public static BindingSettings NetMsmq => new HostBindingSettings(BindingProtocol.NetMsmq)
        {
            HostName = "localhost"
        };

        /// <summary>
        /// Creates msmq.formatname binding
        /// </summary>
        public static BindingSettings MsmqFormatName => new BindingSettings(BindingProtocol.MsmqFormatName)
        {
            HostName = "localhost"
        };
    }
}