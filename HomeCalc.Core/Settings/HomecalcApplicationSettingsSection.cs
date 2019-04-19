using System.Configuration;

namespace HomeCalc.Core.Settings
{
    public class HomecalcApplicationSettingsSection : ConfigurationSection
    {
        public const string SectionName = "homecalcApplicationSettings";

        [ConfigurationProperty("update")]
        public UpdateConfigurationElement Update
        {
            get
            {
                return base["update"] as UpdateConfigurationElement;
            }
        }
    }
}
