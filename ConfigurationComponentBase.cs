using System;
using System.IO;
using System.Reflection;
using HWReflection;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;

public abstract class ConfigurationComponentBase
{
    public void SaveSettings()
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

        string xmlFilePath = Path.Combine(outputFolderPath, "appsettings.xml");
        XDocument xmlDoc = XDocument.Load(xmlFilePath);
        XElement xmlAppSettings = xmlDoc.Element("Settings").Element("ApplicationSettings");

        foreach (PropertyInfo property in GetType().GetProperties())
        {
            var attr = (ConfigurationItemAttribute)Attribute.GetCustomAttribute(property, typeof(ConfigurationItemAttribute));
            if (attr != null)
            {
                switch (attr.ProviderType)
                {
                    case ProviderType.File:
                        XElement settingElement = xmlAppSettings.Element(attr.SettingName);

                        if (property.PropertyType == typeof(TimeSpan))
                        {
                            settingElement.Value = ((TimeSpan)property.GetValue(this)).ToString();
                        }
                        else
                        {
                            settingElement.Value = property.GetValue(this).ToString();
                        }
                        break;
                    case ProviderType.ConfigurationManager:
                        JToken settings = fileContent["ApplicationSettings"];
                        settings[attr.SettingName] = JToken.FromObject(property.GetValue(this));
                        break;
                }
            }
        }

        using (var streamWriter = new StreamWriter(jsonFile.Source.Path))
        {
            streamWriter.Write(fileContent.ToString());
        }

        xmlDoc.Save(xmlFilePath); 
    }

    public void LoadSettings()
    {
        string outputFolderPath = AppDomain.CurrentDomain.BaseDirectory;
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(outputFolderPath)
            .AddJsonFile("appsettings.json");

        var configuration = configurationBuilder.Build();

        string xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.xml");
        XDocument xmlDoc = XDocument.Load(xmlFilePath);
        XElement xmlAppSettings = xmlDoc.Element("Settings").Element("ApplicationSettings");

        foreach (PropertyInfo property in GetType().GetProperties())
        {
            var attr = (ConfigurationItemAttribute)Attribute.GetCustomAttribute(property, typeof(ConfigurationItemAttribute));
            if (attr != null)
            {
                switch (attr.ProviderType)
                {
                    case ProviderType.File:
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
                        break;
                    case ProviderType.ConfigurationManager:
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
                        break;
                }
            }
        }
    }
}