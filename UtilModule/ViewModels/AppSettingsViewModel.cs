using LSlicer.BL.Interaction;
using LSlicer.Infrastructure;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UtilModule.ViewModels
{
    public class AppSettingsViewModel : BindableBase
    {
        private readonly IEventAggregator _ea;
        private SettingsModel _settingsModel;
        private IAppSettings _appSettings;

        public AppSettingsViewModel(IEventAggregator ea)
        {
            _settingsModel = new SettingsModel();
            _ea = ea;
            _ea.GetEvent<AppSettingsSentToViewEvent>()
                .Subscribe(settings => 
                { 
                    _settingsModel.CopyFrom(settings);
                    _appSettings = settings;
                    SettingsPairs = _settingsModel.Get();
                });
        }

        private ObservableCollection<UIPair> _settingsPairs;
        public ObservableCollection<UIPair> SettingsPairs
        {
            get { return _settingsPairs; }
            set { SetProperty(ref _settingsPairs, value); }
        }

        private DelegateCommand _saveSettings;
        public DelegateCommand SaveSettingsCommand =>
            _saveSettings ?? (_saveSettings = new DelegateCommand(ExecuteSaveSettingsCommand));

        void ExecuteSaveSettingsCommand()
        {
            _settingsModel.Set(SettingsPairs);
            _settingsModel.CopyTo(_appSettings);
            _ea.GetEvent<AppSettingsSentToModelEvent>().Publish(_appSettings);
        }
    }
}
