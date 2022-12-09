using LSlicer.BL.Interaction;
using LSlicer.Infrastructure;
using LSlicer.Views;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Unity;

namespace LSlicer.Model
{
    public class SettingsController
    {
        private readonly IAppSettings _appSettings;
        private readonly IUnityContainer _containerProvider;
        private readonly IEventAggregator _ea;
        private readonly ILoggerService _logger;

        private Window _settingsWindow;
        private Window _colorSettingsWindow;
        private Window _additionSettingsWindow;

        public SettingsController(
            IAppSettings appSettings,
            IUnityContainer containerProvider,
            IEventAggregator ea, 
            ILoggerService logger)
        {
            _appSettings = appSettings;
            _containerProvider = containerProvider;
            _ea = ea;
            _ea.GetEvent<AppSettingsSentToModelEvent>().Subscribe(settings => _appSettings.CopyFrom(settings));
            _logger = logger;
            _logger.Info($"[{nameof(SettingsController)}] Initialization " +
                $"\n-- Support engine: {_appSettings.SupportEnginePath} " +
                $"\n-- Slicing engine: {_appSettings.SlicingEnginePath}" +
                $"\n-- Work directory: {_appSettings.WorkingDirectory}" +
                $"\n-- Slicing parameters repo: {_appSettings.SlicingParametersRepoPath}" +
                $"\n-- Result directory: {_appSettings.SlicingResultDirectory}");

        }

        public IList<string> GetSupportEngineList() => _appSettings.SupportEngineList.Split(';').Where(x => x != String.Empty).ToList();
        public IList<string> GetSliceEngineList() => _appSettings.SliceEngineList.Split(';').Where(x => x != String.Empty).ToList();

        public string SelectedSupportEngine
        {
            get => _appSettings.SelectedSupportEngine;
            set => _appSettings.SelectedSupportEngine = value;
        }

        public string SelectedSliceEngine
        {
            get => _appSettings.SelectedSliceEngine;
            set => _appSettings.SelectedSliceEngine = value;
        }

        public string CurrentChangesPath
        {
            get => _appSettings.CurrentChangesPath;
            set => _appSettings.CurrentChangesPath = value;
        }


        public void RaiseSettingsView()
        {
            _settingsWindow = _containerProvider.Resolve<SettingsView>();
            _settingsWindow.Owner = Application.Current.MainWindow;
            _settingsWindow.Show();
            _ea.GetEvent<AppSettingsSentToViewEvent>().Publish(_appSettings);
        }

        public void RaiseColorSettingsView()
        {
            _colorSettingsWindow = _containerProvider.Resolve<SettingsView>();
            _colorSettingsWindow.Owner = Application.Current.MainWindow;
            _colorSettingsWindow.Show();
        }

        public void RaiseAdditionSettingsView()
        {
            _additionSettingsWindow = _containerProvider.Resolve<SettingsView>();
            _additionSettingsWindow.Owner = Application.Current.MainWindow;
            _additionSettingsWindow.Show();
        }

    }
}
