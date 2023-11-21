using HWReflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HWReflection
{
    public interface IConfigurationProvider
    {
        ProviderType ProviderType { get; }

        void LoadSettingsFromSource(PropertyInfo property, MyConfigurationClass configClass);

        void SaveSettingsToSource(PropertyInfo property, MyConfigurationClass configClass);
    }
}
