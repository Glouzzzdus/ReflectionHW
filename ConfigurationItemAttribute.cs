using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HWReflection
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigurationItemAttribute : Attribute
    {
        public string SettingName { get; set; }
        public ProviderType ProviderType { get; set; }

        public ConfigurationItemAttribute(string settingName, ProviderType providerType)
        {
            SettingName = settingName;
            ProviderType = providerType;
        }
    }
}
