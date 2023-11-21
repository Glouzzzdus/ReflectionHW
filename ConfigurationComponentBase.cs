using HWReflection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using IConfigurationProvider = HWReflection.IConfigurationProvider;

public class ConfigurationComponentBase
{
    private IDictionary<ProviderType, IConfigurationProvider> _configurationProviders;

    public ConfigurationComponentBase()
    {
        _configurationProviders = new Dictionary<ProviderType, IConfigurationProvider>();
        LoadPlugins();
    }

    private void LoadPlugins()
    {
        string pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
        if (!Directory.Exists(pluginsPath))
        {
            throw new DirectoryNotFoundException($"Directory '{pluginsPath}' not found.");
        }

        foreach (var file in Directory.GetFiles(pluginsPath, "*.dll"))
        {
            Assembly assembly = Assembly.LoadFrom(file);
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IConfigurationProvider).IsAssignableFrom(type) && !type.IsInterface)
                {
                    IConfigurationProvider providerInstance = (IConfigurationProvider)Activator.CreateInstance(type);
                    _configurationProviders.Add(providerInstance.ProviderType, providerInstance);
                }
            }
        }
    }

    public void LoadSettings(MyConfigurationClass configClass)
    {
        foreach (PropertyInfo property in configClass.GetType().GetProperties())
        {
            ConfigurationItemAttribute attribute = property.GetCustomAttribute(typeof(ConfigurationItemAttribute)) as ConfigurationItemAttribute;
            if (attribute != null)
            {
                if (!_configurationProviders.ContainsKey(attribute.ProviderType))
                {
                    throw new InvalidOperationException($"No provider found for ProviderType '{attribute.ProviderType}'.");
                }

                _configurationProviders[attribute.ProviderType].LoadSettingsFromSource(property, configClass);
            }
        }
    }

    public void SaveSettings(MyConfigurationClass configClass)
    {
        foreach (PropertyInfo property in configClass.GetType().GetProperties())
        {
            ConfigurationItemAttribute attribute = property.GetCustomAttribute(typeof(ConfigurationItemAttribute)) as ConfigurationItemAttribute;
            if (attribute != null)
            {
                if (!_configurationProviders.ContainsKey(attribute.ProviderType))
                {
                    throw new InvalidOperationException($"No provider found for ProviderType '{attribute.ProviderType}'.");
                }
                _configurationProviders[attribute.ProviderType].SaveSettingsToSource(property, configClass);
            }
        }
    }
}