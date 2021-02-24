#region Using Statements

#endregion



namespace Cake.IIS
{
    /// <summary>
    /// Class to set the protocol for a website binding
    /// </summary>
    public class BindingProtocol
    {
        /// <value>
        /// <see cref="BindingProtocol"/> for <c>ftp</c> IIS binding type.
        /// </value>
        public static BindingProtocol Ftp
        {
            get { return new BindingProtocol("ftp"); }
        }

        /// <value>
        /// <see cref="BindingProtocol"/> for <c>http</c> IIS binding type.
        /// </value>
        public static BindingProtocol Http
        {
            get { return new BindingProtocol("http"); }
        }

        /// <value>
        /// <see cref="BindingProtocol"/> for <c>https</c> IIS binding type.
        /// </value>
        public static BindingProtocol Https
        {
            get { return new BindingProtocol("https"); }
        }

        /// <value>
        /// <see cref="BindingProtocol"/> for <c>net.tcp</c> IIS binding type.
        /// </value>
        public static BindingProtocol NetTcp
        {
            get { return new BindingProtocol("net.tcp"); }
        }

        /// <value>
        /// <see cref="BindingProtocol"/> for <c>net.pipe</c> IIS binding type.
        /// </value>
        public static BindingProtocol NetPipe
        {
            get { return new BindingProtocol("net.pipe"); }
        }

        /// <value>
        /// <see cref="BindingProtocol"/> for <c>net.msmq</c> IIS binding type.
        /// </value>
        public static BindingProtocol NetMsmq
        {
            get { return new BindingProtocol("net.msmq"); }
        }

        /// <summary>
        /// <see cref="BindingProtocol"/> for <c>msmq.formatname</c> IIS binding type.
        /// </summary>
        public static BindingProtocol MsmqFormatName
        {
            get { return new BindingProtocol("msmq.formatname"); }
        }



        private BindingProtocol(string name)
        {
            this.Name = name;
        }



        private string Name { get; set; }


        /// <inheritdoc cref="ToString"/>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
