using LaserAprSlicer.Infrastructure.DB;
using LSlicer.BL.Domain;
using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Contracts;
using LSlicer.Helpers;
using LSlicer.Implementations;
using LSlicer.Model;
using LSlicer.ViewModels;
using PluginFramework.CustomPlugin.Installation;
using PluginFramework;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JsonWrapper;
using LSlicer.Data.Interaction.Contracts;
using LSlicer.Data.Interaction;
using LSupportLibrary;
using Prism.Unity;
using SupportManager;
using Unity.Injection;
using LSlicingLibrary;
using SlicingManager;
using Unity.Resolution;
using Unity;
using LSlicer.Data.Model;
using System.IO;
using CommonServiceLocator;
using System.ComponentModel;
using Unity.ServiceLocation;

namespace LSlicer
{
    internal class DependencyInjectionStartPoint
    {
        private readonly IContainerRegistry _containerRegistry;
        private readonly ILoggerService _loggerService;
        private readonly IContainerProvider _container;

        public DependencyInjectionStartPoint(IContainerRegistry containerRegistry, ILoggerService loggerService, IContainerProvider container)
        {
            _containerRegistry = containerRegistry;
            _loggerService = loggerService;
            _container = container;
        }

        public void RegisterDependencies() 
        {
            try
            {
                //AppSettingsContext appSettingsContext = new AppSettingsContext(loggerService);
                //DbCurrentAppSettings dbCurrentAppSettings = new DbCurrentAppSettings(new AppSettingsContext(), AppSettings.DefaultValue);
                AppSettingsLiteDBContext appSettingsContext = new AppSettingsLiteDBContext("settingsNoSql.db");
                //DefaultAppSettings emptyAppSettings = new DefaultAppSettings();
                IAppSettings appSettings = appSettingsContext.GetSettings();
                SetEnginePath(appSettings);
                _containerRegistry.RegisterSingleton<ICloseApplicationHandler, CloseAppHandler>();
                _containerRegistry.RegisterSingleton<IInstallPluginStrategy, ReflectionPluginInstaller>();
                _containerRegistry.RegisterSingleton<IPluginManager, PluginManager>();
                _containerRegistry.RegisterSingleton<IPluginsActivator, ReflectionPluginInstaller>();
                _containerRegistry.RegisterInstance<IDbContext>(appSettingsContext);
                _containerRegistry.RegisterSingleton<SettingsController>();
                _containerRegistry.RegisterSingleton<PresenterModel>();
                _containerRegistry.RegisterSingleton<WorkTaskModel>();
                _containerRegistry.RegisterSingleton<ParametersModel>();
                _containerRegistry.RegisterInstance<ILoggerService>(_loggerService);
                _containerRegistry.RegisterInstance<IAppSettings>(appSettings);
                _containerRegistry.Register<IWorkSaver, LocalResultWorkSaver>();
                _containerRegistry.RegisterSingleton<IPartService, PartService<string>>();
                _containerRegistry.RegisterSingleton<IOperationStack, OperationStack>();
                _containerRegistry.Register<IPartRepository, PartRepository>();
                _containerRegistry.Register<IDispatcher<string>, ProcessingMessageDispatcher<string>>();
                _containerRegistry.RegisterSingleton<DispatcherAggregator<string>>();
                _containerRegistry.Register<IValidator<string>, StlFileValidator>();
                _containerRegistry.Register<IWorkStateManager, LocalWorkStateManager>();
                _containerRegistry.Register<IPartSerializer, PartSerializer>();
                _containerRegistry.Register<IExternalPartManager, VisualScenePartManager>();
                _containerRegistry.Register<ISavedLoader, PartPostProcessLoader>();
                _containerRegistry.RegisterSingleton<IFrontOperationEventAggregator, FrontOperationEventAggregator>();
                _containerRegistry.Register<IPartTransformer, PartTransformer>();
                RegisterSlicingTypes(_containerRegistry, _loggerService);

                RegisterSupportTypes(_containerRegistry, _loggerService);

                SetServiceLocatorProvider();
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Error in {nameof(App)}: ", ex);
                MessageBox.Show("Open error: " + ex.Message);
                throw;
            }
        }

        private void RegisterSupportTypes(IContainerRegistry containerRegistry, ILoggerService loggerService)
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

        private void RegisterSlicingTypes(IContainerRegistry containerRegistry, ILoggerService loggerService)
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
            string parametersRepoPath = PathHelper.Resolve(AppSettingsResourceFile.DefaultValue.SlicingParametersRepoPath);
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
            string parametersRepoPath = PathHelper.Resolve(AppSettingsResourceFile.DefaultValue.SupportParametersRepoPath);
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
        private IUnityContainer SetServiceLocatorProvider()
        {
            var container = _container.GetContainer();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(container));
            return container;
        }
    }
}
