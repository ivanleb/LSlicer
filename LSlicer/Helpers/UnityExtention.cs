using CommonServiceLocator;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Unity;
using Unity.Lifetime;

namespace LSlicer.Helpers
{
    public static class UnityExtention 
    {
        public static void Unregister<T>(T instance)
        {
            IUnityContainer container = ServiceLocator.Current.GetInstance<IUnityContainer>();
            foreach (var registration in container.Registrations.Where(p => p.RegisteredType == typeof(T)
                                                && p.MappedToType == instance.GetType()
                                                && p.LifetimeManager.GetType() == typeof(ContainerControlledLifetimeManager)))
            {
                registration.LifetimeManager.RemoveValue();
            }
        }

        public static UnityConfigurationSection GetConfigSection(string configFileName, string configSectionName)
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo appDirectoryInfo = new DirectoryInfo(appDirectory);
            FileInfo configFileInfo = appDirectoryInfo.GetFiles()
                .FirstOrDefault(file => String.Compare(file.Name, configFileName, true, CultureInfo.InvariantCulture) == 0);

            if (configFileInfo == null)
                throw new FileNotFoundException(configFileName);

            var map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = configFileName;
            var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            var section = (UnityConfigurationSection)config.GetSection(configSectionName);
            return section;
        }

        public static void LoadNewConfiguration(string configFileName, string configSectionName, string containerName)
        {
            UnityConfigurationSection section = GetConfigSection(configFileName, configSectionName);
            IUnityContainer container = ServiceLocator.Current.GetInstance<IUnityContainer>();
            section.Configure(container, containerName);
        }

        public static List<string> GetInstalledPlugins(string configFileName, string configSectionName, string containerName)
        {
            UnityConfigurationSection section = GetConfigSection(configFileName, configSectionName);
            return section.Containers[containerName].Registrations.Select(regItem => regItem.Name).ToList();
        }

        public static XmlNode ConvertToXmlNode(string nodeText)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(nodeText);
            return doc.DocumentElement;
        }

        public static void AddNodeToDocument(string parentNodeTag, string nodeText, XmlDocument document)
        {
            XmlNode typeAliasesElement = document.GetElementsByTagName(parentNodeTag)[0];
            XmlNode newNode = ConvertToXmlNode(nodeText);
            XmlNode importNode = typeAliasesElement.OwnerDocument.ImportNode(newNode, true);
            XmlNode newN = typeAliasesElement.AppendChild(importNode);
        }
    }
}
