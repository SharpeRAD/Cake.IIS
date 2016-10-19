using System;

namespace Cake.IIS
{
    /// <summary>
    /// Class to configure any type of IIS binding (secure or not).
    /// </summary>
    public class BindingSettings
    {
        #region Constructor (2)
        /// <summary>
        /// Creates new instance of <see cref="BindingSettings"/>.
        /// </summary>
        public BindingSettings()
        {

        }

        /// <summary>
        /// Creates new instance of <see cref="BindingSettings"/>.
        /// </summary>
        /// <param name="bindingProtocol">Binding type.</param>
        public BindingSettings(BindingProtocol bindingProtocol)
        {
            this.BindingProtocol = bindingProtocol;

            IISBindings.Http.SetIpAddress("127.0.0.1").SetPort(8080);
        }
        #endregion





        #region Properties (7)
        /// <summary>
        /// Gets or sets IP Address
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets IP Port
        /// </summary>
        public int Port { get; set; }



        /// <summary>
        /// Gets or sets Host Name for binding
        /// </summary>
        public string HostName { get; set; }



        /// <summary>
        /// Gets or sets hash for specific certificate.
        /// </summary>
        public byte[] CertificateHash { get; set; }

        /// <summary>
        /// Gets or sets the name of Certificate Store
        /// </summary>
        public string CertificateStoreName { get; set; }



        /// <summary>
        /// Gets IIS binding type.
        /// </summary>>
        /// <returns>
        /// Returns <see cref="BindingProtocol"/> which will be used to determine IIS binding type.
        /// </returns>
        public BindingProtocol BindingProtocol { get; set; }
        


        // <summary>
        /// Gets IIS binding information
        /// </summary>
        /// <returns>
        /// Returns details of binding properties required to set up specific binding type.
        /// </returns>
        public virtual string BindingInformation
        {
            get
            {
                return string.Format(@"{0}:{1}:{2}", IpAddress, Port, HostName);
            }
        }
        #endregion
    }
}