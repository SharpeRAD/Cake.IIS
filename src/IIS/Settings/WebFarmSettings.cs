namespace Cake.IIS
{
    public class WebFarmSettings
    {
        public WebFarmSettings()
        {
        }

        public string Name { get; set; }

        public string[] Servers { get; set; }

        public bool Overwrite { get; set; }
    }
}