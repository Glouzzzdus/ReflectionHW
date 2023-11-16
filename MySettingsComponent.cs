using System;
using System.Reflection;

namespace HWReflection
{
    public class MySettingsComponent : ConfigurationComponentBase
    {
        [ConfigurationItem("MyFirstSetting", typeof(FileConfigurationProvider))]
        public int MyFirstSetting { get; set; }

        [ConfigurationItem("MySecondSetting", typeof(ConfigurationManagerConfigurationProvider))]
        public string MySecondSetting { get; set; }
    }
}


