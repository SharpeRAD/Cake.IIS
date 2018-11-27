namespace Cake.IIS
{
    /// <summary>
    /// Contains extension methods to configure bindings created by <see cref="BindingExtensions"/>
    /// </summary>
    public static class BindingExtensions
    {
        /// <summary>
        /// Specifies the host name value of the binding.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="hostName">The Host Name.</param>
        /// <returns>The same <see cref="BindingSettings"/> instance so that multiple calls can be chained.</returns>
        public static BindingSettings SetHostName(this BindingSettings binding, string hostName)
        {
            binding.HostName = hostName;
            return binding;
        }

        /// <summary>
        /// Specifies the IP Address value of the binding.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="ipAddress">The IP Address.</param>
        /// <returns>The same <see cref="BindingSettings"/> instance so that multiple calls can be chained.</returns>
        public static BindingSettings SetIpAddress(this BindingSettings binding, string ipAddress)
        {
            binding.IpAddress = ipAddress;
            return binding;
        }

        /// <summary>
        /// Specifies the port number of the binding.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="port">The port number.</param>
        /// <returns>The same <see cref="BindingSettings"/> instance so that multiple calls can be chained.</returns>
        public static BindingSettings SetPort(this BindingSettings binding, int port)
        {
            binding.Port = port;
            return binding;
        }

        /// <summary>
        /// Specifies the certificate store name of the binding.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="certificateStoreName">The certificate store name.</param>
        /// <returns>The same <see cref="BindingSettings"/> instance so that multiple calls can be chained.</returns>
        public static BindingSettings SetCertificateStoreName(this BindingSettings binding, string certificateStoreName)
        {
            binding.CertificateStoreName = certificateStoreName;
            return binding;
        }

        /// <summary>
        /// Specifies the certificate has of the binding.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="certificateHash">The certificate hash.</param>
        /// <returns>The same <see cref="BindingSettings"/> instance so that multiple calls can be chained.</returns>
        public static BindingSettings SetCertificateHash(this BindingSettings binding, byte[] certificateHash)
        {
            binding.CertificateHash = certificateHash;
            return binding;
        }

        /// <summary>
        /// Specifies whether or not Server Name Indication (SNI) is required.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="requireServerNameIndication">Whether or not Server Name Indication should be required</param>
        /// <returns>The same <see cref="BindingSettings"/> instance so that multiple calls can be chained.</returns>
        public static BindingSettings RequireServerNameIndication(this BindingSettings binding, bool requireServerNameIndication)
        {
            binding.RequireServerNameIndication = requireServerNameIndication;
            return binding;
        }
    }
}