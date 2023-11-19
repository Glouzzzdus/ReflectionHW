using System;
using System.IO;
using System.Reflection;
using HWReflection;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Collections.Generic;

public abstract class ConfigurationComponentBase
{
    private Dictionary<ProviderType, Action> loadActions;
    private Dictionary<ProviderType, Action> saveActions;

    public ConfigurationComponentBase()
    {
        loadActions = new Dictionary<ProviderType, Action>
        {
            { ProviderType.File, LoadXmlSettings },
            { ProviderType.ConfigurationManager, LoadJsonSettings }
        };

        saveActions = new Dictionary<ProviderType, Action>
        {
            { ProviderType.File, SaveXmlSettings },
            { ProviderType.ConfigurationManager, SaveJsonSettings }
        };
    }

    public void SaveSettings()
    {
        var providerTypes = GetDistinctProviderTypes();
        foreach (var providerType in providerTypes)
        {
            saveActions[providerType].Invoke();
        }
    }

    public void LoadSettings()
    {
        var providerTypes = GetDistinctProviderTypes();
        foreach (var providerType in providerTypes)
        {
            loadActions[providerType].Invoke();
        }
    }

    private HashSet<ProviderType> GetDistinctProviderTypes()
    {
        return new HashSet<ProviderType>(this.GetType().GetProperties()
            .Select(p => Attribute.GetCustomAttribute(p, typeof(ConfigurationItemAttribute)) as ConfigurationItemAttribute)
            .Where(attr => attr != null)
            .Select(attr => attr.ProviderType)
            .Distinct());
    }

    private void SaveJsonSettings()
    {
        string outputFolderPath = AppDomain.CurrentDomain.BaseDirectory;
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(outputFolderPath)
            .AddJsonFile("appsettings.json");

        var configuration = configurationBuilder.Build();
        var configRoot = configuration as ConfigurationRoot;
        var jsonFile = configRoot.Providers
            .FirstOrDefault(p => p.GetType() == typeof(JsonConfigurationProvider)) as JsonConfigurationProvider;

        if (jsonFile == null)
        {
            throw new InvalidOperationException("appsettings.json was not found.");
        }

        JObject fileContent;

        using (var streamReader = new StreamReader(jsonFile.Source.Path))
        {
            fileContent = JObject.Parse(streamReader.ReadToEnd());
        }

        foreach (PropertyInfo property in GetType().GetProperties())
        {
            var attr = (ConfigurationItemAttribute)Attribute.GetCustomAttribute(property, typeof(ConfigurationItemAttribute));
            if (attr != null && attr.ProviderType == ProviderType.ConfigurationManager)
            {
                JToken settings = fileContent["ApplicationSettings"];
                settings[attr.SettingName] = JToken.FromObject(property.GetValue(this));
            }
        }

        using (var streamWriter = new StreamWriter(jsonFile.Source.Path))
        {
            streamWriter.Write(fileContent.ToString());
        }
    }

    private void SaveXmlSettings()
    {
        string outputFolderPath = AppDomain.CurrentDomain.BaseDirectory;
        string xmlFilePath = Path.Combine(outputFolderPath, "appsettings.xml");
        XDocument xmlDoc = XDocument.Load(xmlFilePath);
        XElement xmlAppSettings = xmlDoc.Element("Settings").Element("ApplicationSettings");

        foreach (PropertyInfo property in GetType().GetProperties())
        {
            var attr = (ConfigurationItemAttribute)Attribute.GetCustomAttribute(property, typeof(ConfigurationItemAttribute));
            if (attr != null && attr.ProviderType == ProviderType.File)
            {
                XElement settingElement = xmlAppSettings.Element(attr.SettingName);

                if (property.PropertyType == typeof(TimeSpan))
                {
                    settingElement.Value = ((TimeSpan)property.GetValue(this)).ToString();
                }
                else
                {
                    settingElement.Value = property.GetValue(this).ToString();
                }
            }
        }

        xmlDoc.Save(xmlFilePath);
    }

    private void LoadJsonSettings()
    {
        string outputFolderPath = AppDomain.CurrentDomain.BaseDirectory;
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(outputFolderPath)
            .AddJsonFile("appsettings.json");

        var configuration = configurationBuilder.Build();

        foreach (PropertyInfo property in GetType().GetProperties())
        {
            var attr = (ConfigurationItemAttribute)Attribute.GetCustomAttribute(property, typeof(ConfigurationItemAttribute));
            if (attr != null && attr.ProviderType == ProviderType.ConfigurationManager)
            {
                string configRawValue = configuration[$"ApplicationSettings:{attr.SettingName}"];

                if (property.PropertyType == typeof(TimeSpan))
                {
                    if (TimeSpan.TryParse(configRawValue, out TimeSpan timeSpanValue))
                    {
                        property.SetValue(this, timeSpanValue);
                    }
                    else
                    {
                        throw new ArgumentException($"Can't convert '{configRawValue}' property {property.Name} to TimeSpan type.");
                    }
                }
                else
                {
                    object value = Convert.ChangeType(configRawValue, property.PropertyType);
                    property.SetValue(this, value);
                }
            }
        }
    }

    private void LoadXmlSettings()
    {
        string outputFolderPath = AppDomain.CurrentDomain.BaseDirectory;
        string xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.xml");
        XDocument xmlDoc = XDocument.Load(xmlFilePath);
        XElement xmlAppSettings = xmlDoc.Element("Settings").Element("ApplicationSettings");

        foreach (PropertyInfo property in GetType().GetProperties())
        {
            var attr = (ConfigurationItemAttribute)Attribute.GetCustomAttribute(property, typeof(ConfigurationItemAttribute));
            if (attr != null && attr.ProviderType == ProviderType.File)
            {
                string rawValue = xmlAppSettings.Element(attr.SettingName).Value;

                if (property.PropertyType == typeof(TimeSpan))
                {
                    if (TimeSpan.TryParse(rawValue, out TimeSpan timeSpanValue))
                    {
                        property.SetValue(this, timeSpanValue);
                    }
                    else
                    {
                        throw new ArgumentException($"Can't convert '{rawValue}' property {property.Name} to TimeSpan type.");
                    }
                }
                else
                {
                    object value = Convert.ChangeType(rawValue, property.PropertyType);
                    property.SetValue(this, value);
                }
            }
        }
    }
}