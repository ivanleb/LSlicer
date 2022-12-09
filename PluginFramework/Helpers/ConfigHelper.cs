using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace PluginFramework.Installation
{
    internal static class ConfigHelper
    {
        internal static void AddInstalledPluginToDescription(IEnumerable<PluginConfig> pluginConfigs)
        {
            string pluginListPath = GetInstalledPluginListPath();

            PluginConfig[] configs = GetInstalledPluginsFromDescription(pluginListPath);

            List<PluginConfig> newDescriptions = new List<PluginConfig>();
            newDescriptions.AddRange(configs);
            newDescriptions.AddRange(pluginConfigs);
            WritePluginDescriptions(newDescriptions, pluginListPath);
        }

        internal static PluginConfig[] GetInstalledPluginsFromDescription(string pluginListPath)
        {
            return File.Exists(pluginListPath)
                ? ReadPluginDescriptions(pluginListPath)
                : new PluginConfig[0];
        }

        internal static string GetInstalledPluginListPath()
        {
            return Path.Combine(AppContext.BaseDirectory, PluginValues.PluginsDirectoryName, PluginValues.InstalledPluginsConfig);
        }

        internal static void WriteXmlDescription(IEnumerable<PluginConfig> pluginConfigs, string resultDirPath)
        {
            WriteXmlDescription(pluginConfigs, new DirectoryInfo(resultDirPath));
        }
        internal static void WriteXmlDescription(IEnumerable<PluginConfig> pluginConfigs, DirectoryInfo resultDir)
        {
            string pluginDescriptionPath = Path.Combine(resultDir.FullName, PluginValues.DescriptionFileName);
            WritePluginDescriptions(pluginConfigs, pluginDescriptionPath);
        }

        private static void WritePluginDescriptions(IEnumerable<PluginConfig> pluginConfigs, string pluginDescriptionPath)
        {
            using (TextWriter writer = new StreamWriter(pluginDescriptionPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PluginConfig[]));
                serializer.Serialize(writer, pluginConfigs.ToArray());
                writer.Close();
            }
        }

        internal static PluginConfig[] ReadPluginDescriptions(DirectoryInfo pluginDirectoryInfo)
        {
            string descriptionFilePath = Path.Combine(pluginDirectoryInfo.FullName, PluginValues.DescriptionFileName);
            return ReadPluginDescriptions(descriptionFilePath);
        }

        internal static void RemovePluginFromDescriptions(IPlugin plugin)
        {
            string installedPluginPath = GetInstalledPluginListPath();
            List<PluginConfig> plugins = ReadPluginDescriptions(installedPluginPath).ToList();
            var configToRemove = plugin.CreateConfig();
            List<PluginConfig> filteredConfigs = plugins.Where(plug => !plug.TheSame(configToRemove)).ToList();
            WritePluginDescriptions(filteredConfigs, installedPluginPath);

        }

        internal static PluginConfig[] ReadPluginDescriptions(string descriptionFilePath)
        {
            using (TextReader reader = new StreamReader(File.OpenRead(descriptionFilePath)))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PluginConfig[]));
                PluginConfig[] configs = (PluginConfig[])serializer.Deserialize(reader);
                reader.Close();
                return configs;
            }
        }
    }
}
