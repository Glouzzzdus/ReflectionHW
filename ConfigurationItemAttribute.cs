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
        public string SettingName { get; }
        public Type ProviderType { get; }

        public ConfigurationItemAttribute(string settingName, Type providerType)
        {
            SettingName = settingName;
            ProviderType = providerType;
        }
    }
}
