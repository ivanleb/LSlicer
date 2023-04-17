using LSlicer.BL.Interaction;
using LSlicer.Helpers;
using LSlicer.Infrastructure;
using LSlicer.Views;
using PluginFramework.Core;
using PluginFramework.CustomPlugin.Helpers;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Unity;

namespace LSlicer.Model
{
    public class PluginController
    {
        private readonly ILoggerService _logger;
        private readonly IEventAggregator _ea;
        private readonly IUnityContainer _containerProvider;
        private readonly IPluginManager _pluginManager;
        private readonly ICloseApplicationHandler _closeApplicationHandler;

        private Window _pluginWindow;

        public PluginController(
            IUnityContainer containerProvider,
            IEventAggregator ea,
            ILoggerService logger,
            IPluginManager pluginManager, 
            ICloseApplicationHandler closeApplicationHandler)
        {
            _logger = logger;
            _ea = ea;
            EventSubscribes();
            _containerProvider = containerProvider;
            _pluginManager = pluginManager;
            _closeApplicationHandler = closeApplicationHandler;
            _closeApplicationHandler.Add(
                "Delete uninstalled plugin files",
                () => FileHelper.FilesToRemove.ForEach(File.Delete));
        }

        public void RaisePluginWindow()
        {
            _pluginWindow = _containerProvider.Resolve<PluginControlPanel>();
            _pluginWindow.Owner = Application.Current.MainWindow;
            _pluginWindow.Show();
        }

        private void InstallPlugin(string pluginPackagePack)
        {
            ActionHelper.SafeExecutionWithLogging(_logger, _pluginManager.GetInstallStrategy().Install, pluginPackagePack);
            InitializePluginCollection();
        }

        private void UninstallPlugin(IPlugin plugin)
        {
            ActionHelper.SafeExecutionWithLogging(_logger, _pluginManager.GetInstallStrategy().Uninstall, plugin);
            InitializePluginCollection();
        }

        private void MakePluginPackage((string pluginPath, string packagePath) paths)
        {
            ActionHelper.SafeExecutionWithLogging(_logger, _pluginManager.GetInstallStrategy().MakePluginPackage, paths.pluginPath, paths.packagePath);
        }

        private void InitializePluginCollection()
        {
            IEnumerable<IPlugin> pluginsContainer = new List<IPlugin>();
            try
            {
                pluginsContainer = _pluginManager.GetPlugins()
                                   .ForEach(plugin => _logger.Info($"[{nameof(PluginController)}] Load plugin \"{plugin.Name}\", remote: '/*RemotingServices.IsTransparentProxy(plugin)*/' "));
            }
            catch (Exception e)
            {
                _logger.Error($"[{nameof(PluginController)}]", e);
                ActionHelper.ShowError(e);
            }
            finally 
            {
                _ea.GetEvent<SendPluginEvent>().Publish(pluginsContainer);
            }
        }

        private void EventSubscribes()
        {
            _ea.GetEvent<PullPluginEvent>().Subscribe(InitializePluginCollection);
            _ea.GetEvent<InstallPluginEvent>().Subscribe(InstallPlugin);
            _ea.GetEvent<MakePluginEvent>().Subscribe(MakePluginPackage);
            _ea.GetEvent<UninstallPluginEvent>().Subscribe(UninstallPlugin);
            _ea.GetEvent<SetDeleteUnusedFilesEvent>().Subscribe(isDelete => FileHelper.DeleteFilesAfterClosingApp = isDelete);
        }
    }
}
