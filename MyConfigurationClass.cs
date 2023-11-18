using System;
using System.Reflection;

namespace HWReflection
{
    public class MyConfigurationClass : ConfigurationComponentBase
    {
        [ConfigurationItem("Setting1", ProviderType.ConfigurationManager)]
        public string? Setting1 { get; set; }

        [ConfigurationItem("Setting2", ProviderType.ConfigurationManager)]
        public int Setting2 { get; set; }

        [ConfigurationItem("Setting3", ProviderType.ConfigurationManager)]
        public double Setting3 { get; set; }

        [ConfigurationItem("Setting4", ProviderType.ConfigurationManager)]
        public TimeSpan Setting4 { get; set; }

        [ConfigurationItem("Setting5", ProviderType.ConfigurationManager)]
        public DateTime Setting5 { get; set; }

        [ConfigurationItem("Setting6", ProviderType.File)]
        public string? Setting6 { get; set; }

        [ConfigurationItem("Setting7", ProviderType.File)]
        public int Setting7 { get; set; }

        [ConfigurationItem("Setting8", ProviderType.File)]
        public double Setting8 { get; set; }

        [ConfigurationItem("Setting9", ProviderType.File)]
        public TimeSpan Setting9 { get; set; }

        [ConfigurationItem("Setting10", ProviderType.File)]
        public DateTime Setting10 { get; set; }
    }
}


