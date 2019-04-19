using System.Configuration;

namespace HomeCalc.Core.Settings
{
    public class UpdateConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("folder")]
        public string Folder
        {
            get
            {
                return base["folder"] as string;
            }
        }

        [ConfigurationProperty("binaryFileName")]
        public string BinaryFileName
        {
            get
            {
                return base["binaryFileName"] as string;
            }
        }

        [ConfigurationProperty("versionFileName")]
        public string VersionFileName
        {
            get
            {
                return base["versionFileName"] as string;
            }
        }
    }
}
