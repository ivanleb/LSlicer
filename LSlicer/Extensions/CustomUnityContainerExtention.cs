using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Builder;
using Unity.Extension;
using Unity.Strategies;

namespace LSlicer.Extensions
{
    public class CustomUnityContainerExtention : UnityContainerExtension, IDisposable
    {
        private MyBuildup strategy = new MyBuildup();
        private bool disposed;

        protected override void Initialize()
        {
            Context.Strategies.Add(strategy, UnityBuildStage.PreCreation);
        }

        /// <summary>
        /// Build the new appdomain allowing 
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="shadowFiles"></param>
        public void CreateAppDomain(string domain, string shadowFiles)
        {
            AppDomainSetup appDomainSetup = new AppDomainSetup();
            appDomainSetup.ApplicationBase = Environment.CurrentDirectory + @"\" + shadowFiles;
            appDomainSetup.ShadowCopyFiles = "true";

            strategy.AppDomain = AppDomain.CreateDomain(domain, null, appDomainSetup);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                AppDomain.Unload(strategy.AppDomain);
                disposed = true;
            }
        }

        #endregion

    }
    public class MyBuildup : BuilderStrategy
    {
        public AppDomain AppDomain { set; get; }

        public void PreBuildUp(BuilderContext context)
        {
            Type targetType = context.Type;

            if (AppDomain != null)
            {
                context.Existing = AppDomain.CreateInstanceAndUnwrap(targetType.Assembly.FullName, targetType.ToString());
                context.BuildComplete = true;
            }
        }

        public void PostBuildUp(BuilderContext context) { }
        public void PreTearDown(BuilderContext context) { }
        public void PostTearDown(BuilderContext context) { }
    }

    public class CustomExtensionConfigHandler : ExtensionConfigurationElement
    {
        /// <summary>
        /// Name of the non-default application domain
        /// </summary>
        [ConfigurationProperty("domain")]
        public string Domain
        {
            get { return (string)this["domain"]; }
            set { this["domain"] = value; }
        }

        /// <summary>
        /// Relative location of replaceable assemblies
        /// </summary>
        [ConfigurationProperty("shadowFiles")]
        public string ShadowFiles
        {
            get { return (string)this["shadowFiles"]; }
            set { this["shadowFiles"] = value; }
        }


        protected override void ConfigureContainer(IUnityContainer container)
        {
            container.Configure<CustomUnityContainerExtention>().CreateAppDomain(Domain, ShadowFiles);
        }
    }
}
