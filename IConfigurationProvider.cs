using HWReflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionHW
{
    public interface IConfigurationProvider
    {
        ProviderType ProviderType { get; }
        void LoadSettingsFromSource(MyConfigurationClass configClass);
        void SaveSettingsToSource(MyConfigurationClass configClass);
    }
}
