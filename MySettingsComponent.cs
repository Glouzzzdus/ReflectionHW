using System;
using System.Reflection;

namespace HWReflection
{
    public class MySettingsComponent : ConfigurationComponentBase
    {
        [ConfigurationItem("Setting1", ProviderType.File)]
        public string MySetting { get; set; }
    }
}


