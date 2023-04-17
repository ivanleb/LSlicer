using PluginFramework.Core;
using PluginFramework.Implementations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PluginFramework.Core.Configuration
{
    public class PluginXMLConfigStorage : IPluginConfigStorage
    {
        public void AddInstalledPluginToConfigStorage(IEnumerable<PluginConfig> pluginConfigs)
        {
            PluginConfig[] configs = GetInstalledPluginsFromDescription();
            List<PluginConfig> newDescriptions = new List<PluginConfig>();
            newDescriptions.AddRange(configs);
            newDescriptions.AddRange(pluginConfigs);
            WritePluginDescriptions(newDescriptions, GetInstalledPluginListPath());
        }

        public PluginConfig[] GetInstalledPluginsFromDescription()
        {
            string pluginListPath = GetInstalledPluginListPath();
            return File.Exists(pluginListPath)
                ? ReadPluginDescriptions(pluginListPath)
                : Array.Empty<PluginConfig>();
        }

        public PluginConfig[] ReadPluginConfigs(DirectoryInfo pluginDirectoryInfo)
        {
            string descriptionFilePath = Path.Combine(pluginDirectoryInfo.FullName, PluginValues.DescriptionFileName);
            return ReadPluginDescriptions(descriptionFilePath);
        }

        public void RemovePluginFromDescriptions(IPlugin plugin)
        {
            string installedPluginPath = GetInstalledPluginListPath();
            List<PluginConfig> plugins = ReadPluginDescriptions(installedPluginPath).ToList();
            var configToRemove = plugin.CreateConfig();
            List<PluginConfig> filteredConfigs = plugins.Where(plug => !plug.TheSame(configToRemove)).ToList();
            WritePluginDescriptions(filteredConfigs, installedPluginPath);
        }

        public void CreatePluginConfig(IEnumerable<PluginConfig> pluginConfigs, DirectoryInfo resultDir)
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

        private static PluginConfig[] ReadPluginDescriptions(string descriptionFilePath)
        {
            using (TextReader reader = new StreamReader(File.OpenRead(descriptionFilePath)))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PluginConfig[]));
                PluginConfig[] configs = (PluginConfig[])serializer.Deserialize(reader);
                reader.Close();
                return configs;
            }
        }

        private string GetInstalledPluginListPath()
        {
            return Path.Combine(AppContext.BaseDirectory, PluginValues.PluginsDirectoryName, PluginValues.InstalledPluginsConfig);
        }
    }
}
