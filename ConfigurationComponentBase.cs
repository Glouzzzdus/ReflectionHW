using System;
using System.IO;
using System.Reflection;

namespace HWReflection
{
    public abstract class ConfigurationComponentBase
    {
        public void SaveSettings()
        {
            foreach (PropertyInfo property in this.GetType().GetProperties())
            {
                var attr = (ConfigurationItemAttribute)Attribute.GetCustomAttribute(property, typeof(ConfigurationItemAttribute));
                if (attr != null)
                {
                    switch (attr.ProviderType)
                    {
                        case ProviderType.File:
                            // Запись в файл логики
                            break;
                        case ProviderType.ConfigurationManager:
                            // Реализация записи через ConfigurationManager
                            break;
                    }

                }
            }
        }

        public void LoadSettings()
        {
            // Аналогично SaveSettings(), но наоборот - читаем значение из источника
        }
    }
    
}

