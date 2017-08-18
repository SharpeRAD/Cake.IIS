namespace Cake.IIS
{
    public class AuthorizationSettings
    {
        public AuthorizationSettings()
        {
            AuthorizationType = AuthorizationType.AllUsers;

            CanRead = true;
            CanWrite = true;
        }

        public AuthorizationType AuthorizationType { get; set; }

        public string[] Users { get; set; }

        public string[] Roles { get; set; }

        public bool CanRead { get; set; }

        public bool CanWrite { get; set; }
    }
}