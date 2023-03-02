using LSlicer.BL.Interaction;
using LSlicer.Properties;
using System;
using System.Linq;
using System.Xml;

namespace LSlicer.Implementations
{
    public class AppSettings : Settings, IAppSettings
    {
        private static AppSettings defaultInstance = ((AppSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new AppSettings())));

        public static AppSettings DefaultValue
        {
            get
            {
                return defaultInstance;
            }
        }

        public override void Save()
        {
            base.Save();
            var properties = typeof(AppSettings).GetProperties();
            XmlDocument xml = new XmlDocument();
            //xml.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            properties.Skip(1).ToList().ForEach(property =>
            {
                XmlNode node = xml.SelectSingleNode($"configuration/userSettings/LaserAprBuildProcessor.Properties.Settings/setting[@name='{property.Name}']");
                if (node != null)
                    node.ChildNodes[0].InnerText = property.GetValue(this).ToString();
            });
           // xml.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }

        public void SetForUser(int id)
        {
            throw new NotImplementedException();
        }
    }
}
