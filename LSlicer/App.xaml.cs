using log4net;
using log4net.Config;
using log4net.Repository;
using LSlicer.BL.Interaction;
using LSlicer.Implementations;
using LSlicer.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Unity;

namespace LSlicer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private DependencyInjectionStartPoint _dependencyInjectionStartPoint;

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ShowPreviewScreen();
            ILoggerRepository repository = LogManager.GetRepository(Assembly.GetCallingAssembly());
            XmlConfigurator.Configure(repository, new FileInfo("log4net.xml"));

            ILog logger = LogManager.GetLogger(Assembly.GetCallingAssembly()/*"LogRepo"*/, "Logger");
            var loggerService = new LoggerService(logger);

            if(_dependencyInjectionStartPoint == null)
                _dependencyInjectionStartPoint = new DependencyInjectionStartPoint(containerRegistry, loggerService, Container);

            _dependencyInjectionStartPoint.RegisterDependencies();
        }

        protected override Window CreateShell()
        {
            var logger = LogManager.GetLogger(Assembly.GetCallingAssembly()/*"LogRepo"*/, "Logger");
            try
            {
                var container = Container.GetContainer() as UnityContainer;
                container.EnableDebugDiagnostic();
                return Container.Resolve<Shell>();
            }
            catch (Exception ex)
            {
                logger.Error($"Error in {nameof(App)}: ", ex);
                MessageBox.Show("Open error: " + ex.Message);
                throw;
            }
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new ConfigurationModuleCatalog();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var container = Container.GetContainer();
            var logger = container.Resolve<ILoggerService>();
            var settings = container.Resolve<IAppSettings>();
            var handler = container.Resolve<ICloseApplicationHandler>();
            try
            {
                /*
                var directoryName = settings.WorkingDirectory;
                if (Directory.Exists(directoryName))
                {
                    string[] filePaths = Directory.GetFiles(directoryName);
                    foreach (string filePath in filePaths)
                        File.Delete(filePath);
                }
                */
                logger.Info($"Close the program.");
                base.OnExit(e);
            }
            catch (Exception ex)
            {
                logger.Error($"Error in {nameof(App)}: ", ex);
                MessageBox.Show("Closing error: " + ex.Message);
                throw;
            }
            finally 
            {
                handler.Handle();
            }
        }

        private static void ShowPreviewScreen()
        {
            String image = @"Resources/impossible-triangle.png";
            try
            {
                SplashScreen splash = new SplashScreen(image);
                splash.Show(false, true);
                splash.Close(TimeSpan.FromSeconds(5));
            }
            catch { }
        }
    }
}
