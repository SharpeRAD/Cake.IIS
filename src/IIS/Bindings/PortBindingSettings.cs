namespace Cake.IIS.Bindings
{
    /// <summary>
    /// Class to configure net.tcp binding.
    /// </summary>
    public class PortBindingSettings : BindingSettings
    {
        /// <summary>
        /// Creates new predefined instance of <see cref="PortBindingSettings"/>.
        /// </summary>
        public PortBindingSettings(BindingProtocol bindingProtocol)
            : base(bindingProtocol)
        {
        }

        /// <inheritdoc cref="BindingSettings.BindingInformation"/>
        public override string BindingInformation => $"{Port}:{HostName}";
    }
}