using CommonServiceLocator;
using JsonWrapper;
using LSlicer.BL.Domain;
using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Contracts;
using LSlicer.Data.Interaction;
using LSlicer.Helpers;
using LSlicer.Implementations;
using LSlicer.Model;
using LSlicer.ViewModels;
using LSlicer.Views;
using LSlicer.Data.Interaction.Contracts;
using LSlicingLibrary;
using LSupportLibrary;
using log4net;
using log4net.Config;
using PluginFramework;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using SlicingManager;
using SupportManager;
using System;
using System.IO;
using System.Windows;
using Unity;
using Unity.Injection;
using Unity.Resolution;
using Unity.ServiceLocation;
using log4net.Repository;
using System.Reflection;
using PluginFramework.CustomPlugin.Installation;

namespace LSlicer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ShowPreviewScreen();
            ILoggerRepository repository = LogManager.GetRepository(Assembly.GetCallingAssembly());
            XmlConfigurator.Configure(repository, new FileInfo("log4net.xml"));

            var logger = LogManager.GetLogger("LogRepo", "Logger");
            var loggerService = new LoggerService(logger);
            try
            {
                AppSettingsContext appSettingsContext = new AppSettingsContext(loggerService);
                DbCurrentAppSettings dbCurrentAppSettings = new DbCurrentAppSettings(appSettingsContext, AppSettings.DefaultValue);

                SetEnginePath(dbCurrentAppSettings);
                containerRegistry.RegisterSingleton<ICloseApplicationHandler, CloseAppHandler>();
                containerRegistry.RegisterSingleton<IInstallPluginStrategy, ReflectionPluginInstaller>();
                containerRegistry.RegisterSingleton<IPluginManager, PluginManager>();
                containerRegistry.RegisterSingleton<IPluginsActivator, ReflectionPluginInstaller>();
                containerRegistry.RegisterInstance(appSettingsContext);
                containerRegistry.RegisterSingleton<UserIdentityController>();
                containerRegistry.RegisterSingleton<IdentityController>();
                containerRegistry.RegisterSingleton<SettingsController>();
                containerRegistry.RegisterSingleton<PresenterModel>();
                containerRegistry.RegisterSingleton<WorkTaskModel>();
                containerRegistry.RegisterSingleton<ParametersModel>();
                containerRegistry.RegisterInstance<ILoggerService>(loggerService);
                containerRegistry.RegisterInstance<IAppSettings>(dbCurrentAppSettings);
                containerRegistry.Register<IWorkSaver, LocalResultWorkSaver>();
                containerRegistry.RegisterSingleton<IPartService, PartService<string>>();
                containerRegistry.RegisterSingleton<IOperationStack, OperationStack>();
                containerRegistry.Register<IPartRepository, PartRepository>();
                containerRegistry.Register<IDispatcher<string>, ProcessingMessageDispatcher<string>>();
                containerRegistry.RegisterSingleton<DispatcherAggregator<string>>();
                containerRegistry.Register<IValidator<string>, StlFileValidator>();
                containerRegistry.Register<IWorkStateManager, LocalWorkStateManager>();
                containerRegistry.Register<IPartSerializer, PartSerializer>();
                containerRegistry.Register<IExternalPartManager, VisualScenePartManager>();
                containerRegistry.Register<ISavedLoader, PartPostProcessLoader>();
                containerRegistry.RegisterSingleton<IFrontOperationEventAggregator, FrontOperationEventAggregator>();
                containerRegistry.Register<IPartTransformer, PartTransformer>();
                RegisterSlicingTypes(containerRegistry, loggerService);

                RegisterSupportTypes(containerRegistry, loggerService);

                SetServiceLocatorProvider();
            }
            catch (Exception ex)
            {
                logger.Error($"Error in {nameof(App)}: ", ex);
                MessageBox.Show("Open error: " + ex.Message);
                throw;
            }
        }

        private void RegisterSupportTypes(IContainerRegistry containerRegistry, LoggerService loggerService)
        {
            //containerRegistry.Register<ISupportGenerator<string>, SupportGenerator<string>>();

            containerRegistry.RegisterSingleton<ISupportService, SupportService<string>>();

            containerRegistry.RegisterSingleton<ISupportParametersService, SupportParametersService>();

            containerRegistry.Register<ISetParametersProvider<ISupportParameters>, JsonSetSupportParametersProvider>();
            containerRegistry.Register<IParametersProvider<ISupportParameters>, JsonSetSupportParametersProvider>();

            containerRegistry.RegisterInstance<ISupportParametersRepository>(new JsonSupportParametersRepository(GetSupportRepositoryFileInfo(), loggerService));

            containerRegistry.RegisterSingleton<SupportParameters>();

            containerRegistry.Register<IEngineResultAwaiter, SupportEngineTaskAwaiter>(nameof(SupportEngineTaskAwaiter));
            containerRegistry.Register<IEngineInvoker<string>, SupportEngineInvoker<string>>(nameof(SupportEngineInvoker<string>));

            containerRegistry.Register<ISupportGenerator, CustomSupportGenerator>(nameof(CustomSupportGenerator));
            containerRegistry.GetContainer()
                .RegisterType<ISupportGenerator>(nameof(SupportGenerator<string>), new InjectionFactory(x =>
                   x.Resolve<SupportGenerator<string>>(
                       new DependencyOverride<IEngineInvoker<string>>(x.Resolve<IEngineInvoker<string>>(nameof(SupportEngineInvoker<string>))),
                       new DependencyOverride<IEngineResultAwaiter>(x.Resolve<IEngineResultAwaiter>(nameof(SupportEngineTaskAwaiter))))));

            containerRegistry.Register<IGeneratorHive<ISupportGenerator>, SupportGeneratorHive>();
            containerRegistry.Register<IPostProcessor<IPart>, PartPostProcessLoader>();
        }

        private void RegisterSlicingTypes(IContainerRegistry containerRegistry, LoggerService loggerService)
        {
            containerRegistry.RegisterSingleton<ISliceService, SliceService>();
            containerRegistry.RegisterSingleton<ISlicingParametersService, SlicingParametersService>();
            //containerRegistry.Register<ISliceGenerator<string>, SliceGenerator<string>>();
            containerRegistry.Register<ISliceService, SliceService>();

            containerRegistry.Register<IEngineInvoker<string>, SliceEngineInvoker<string>>(nameof(SliceEngineInvoker<string>));
            containerRegistry.Register<ISetParametersProvider<ISlicingParameters>, JSONSetSlicingParametersProvider>();
            containerRegistry.Register<IParametersProvider<ISlicingParameters>, JSONSetSlicingParametersProvider>();
            containerRegistry.RegisterInstance<ISlicingParametersRepository>(new JsonSlicingParametersRepository(GetSlicingRepositoryFileInfo(), loggerService));
            containerRegistry.RegisterSingleton<SlicingParameters>();
            containerRegistry.Register<IEngineResultAwaiter, SlicingEngineTaskAwaiter>(nameof(SlicingEngineTaskAwaiter));

            containerRegistry.Register<ISliceGenerator, CustomSliceGenerator>(nameof(CustomSliceGenerator));
            containerRegistry.GetContainer()
                .RegisterType<ISliceGenerator>(nameof(SliceGenerator<string>), new InjectionFactory(x =>
                   x.Resolve<SliceGenerator<string>>(
                       new DependencyOverride<IEngineInvoker<string>>(x.Resolve<IEngineInvoker<string>>(nameof(SliceEngineInvoker<string>))),
                       new DependencyOverride<IEngineResultAwaiter>(x.Resolve<IEngineResultAwaiter>(nameof(SlicingEngineTaskAwaiter))))));

            containerRegistry.Register<IGeneratorHive<ISliceGenerator>, SliceGeneratorHive>();
            containerRegistry.Register<IPostProcessor<ISlicingInfo>, SlicingInfoLoader>();
        }

        protected override Window CreateShell()
        {
            var logger = LogManager.GetLogger("LogRepo", "Logger");
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

        private FileInfo GetSlicingRepositoryFileInfo()
        {
#if DEBUG
            var debugPath = "..\\..\\..\\Resources\\DebugSlicingParameters.json";
            var file = new FileInfo(debugPath);
            if (!file.Exists)
            {
                throw new FileNotFoundException(debugPath + "  " + System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            return file;
#endif
            string parametersRepoPath = PathHelper.Resolve(AppSettings.DefaultValue.SlicingParametersRepoPath);
            string parametersFile = Path.Combine(parametersRepoPath, "DebugSlicingParameters.json");
            string defaultParametersPath = @"Resources\DebugSlicingParameters.json";
            Helpers.AppFileHelper.CopyIfDoesntExist(defaultParametersPath, parametersFile);
            return new FileInfo(parametersFile);
        }

        private FileInfo GetSupportRepositoryFileInfo()
        {
#if DEBUG
            var debugPath = "..\\..\\..\\Resources\\DebugSupportParameters.json";
            var file = new FileInfo(debugPath);
            if (!file.Exists)
            {
                throw new FileNotFoundException(debugPath + "  " + System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            return file;
#endif
            string parametersRepoPath = PathHelper.Resolve(AppSettings.DefaultValue.SupportParametersRepoPath);
            string parametersFile = Path.Combine(parametersRepoPath, "DebugSupportParameters.json");
            string defaultParametersPath = @"Resources\DebugSupportParameters.json";
            Helpers.AppFileHelper.CopyIfDoesntExist(defaultParametersPath, parametersFile);
            return new FileInfo(parametersFile);
        }

        private void SetEnginePath(IAppSettings appSettings)
        {
#if DEBUG
            return;
#endif
            if (appSettings.SupportEnginePath.Contains("SupportEngine") &&
                appSettings.SupportEnginePath.Contains("bin") &&
                appSettings.SupportEnginePath.Contains("x64") &&
                appSettings.SupportEnginePath.Contains("Debug"))
                appSettings.SupportEnginePath = "SupportEngine.exe";

            if (appSettings.SlicingEnginePath.Contains("TestEngineStub"))
                appSettings.SlicingEnginePath = "SliceEngine.exe";
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

        private IUnityContainer SetServiceLocatorProvider()
        {
            var container = Container.GetContainer();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            return container;
        }
    }
}
