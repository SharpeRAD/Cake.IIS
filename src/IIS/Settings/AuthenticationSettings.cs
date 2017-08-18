namespace Cake.IIS
{
    public class AuthenticationSettings
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public bool EnableBasicAuthentication { get; set; }

        public bool EnableWindowsAuthentication { get; set; }

        public bool EnableAnonymousAuthentication { get; set; }
    }
}