using LSlicer.Infrastructure;
using Microsoft.Win32;
using PluginFramework;
using PluginManagementModule.Model;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace PluginManagementModule.ViewModels
{
    public class PluginControllerUIViewModel : BindableBase
    {
        private readonly IEventAggregator _ea;
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        public PluginControllerUIViewModel(IEventAggregator ea)
        {
            _ea = ea;
            _ea.GetEvent<SendPluginEvent>().Subscribe(FillPluginList);
        }

        private UIPluginRepresentation _selectedPlugin;
        public UIPluginRepresentation SelectedPlugin
        {
            get { return _selectedPlugin; }
            set { SetProperty(ref _selectedPlugin, value); }
        }

        public ObservableCollection<UIPluginRepresentation> _pluginsList;
        public ObservableCollection<UIPluginRepresentation> PluginsList
        {
            get
            {
                if (_pluginsList == null)
                {
                    InitializePluginCollection();
                    _autoResetEvent.WaitOne();
                }
                return _pluginsList;
            }

            set 
            { 
               SetProperty(ref _pluginsList, value); 
            }
        }

        private bool _deleteAllPluginFiles;
        public bool DeleteAllPluginFiles
        {
            get => _deleteAllPluginFiles; 
            set 
            { 
                SetProperty(ref _deleteAllPluginFiles, value);
                _ea.GetEvent<SetDeleteUnusedFilesEvent>().Publish(_deleteAllPluginFiles);
            }
        }

        private DelegateCommand _runPluginCommand;
        public DelegateCommand RunPluginCommand =>
            _runPluginCommand ?? (_runPluginCommand = new DelegateCommand(() => SelectedPlugin.RunPlugin(), () => SelectedPlugin != null)
            .ObservesProperty(() => SelectedPlugin));

        private DelegateCommand _installPluginCommand;
        public DelegateCommand InstallPluginCommand =>
            _installPluginCommand ?? (_installPluginCommand = new DelegateCommand(ExecuteInstallPluginCommand));

        void ExecuteInstallPluginCommand()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "Zip files (*.zip)|*.zip";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                _ea.GetEvent<InstallPluginEvent>().Publish(openFileDialog.FileName);
            }
        }

        private DelegateCommand _makePluginCommand;
        public DelegateCommand MakePluginCommand =>
            _makePluginCommand ?? (_makePluginCommand = new DelegateCommand(ExecuteMakePluginCommand));

        void ExecuteMakePluginCommand()
        {
            string pluginPath = WindowHelper.ChooseFolder(AppDomain.CurrentDomain.BaseDirectory, "Choose plugin folder");
            string packagePath = WindowHelper.ChooseFolder(AppDomain.CurrentDomain.BaseDirectory, "Choose target package folder");
            _ea.GetEvent<MakePluginEvent>().Publish((pluginPath, packagePath));
        }

        private void InitializePluginCollection() => _ea.GetEvent<PullPluginEvent>().Publish();
        
        private void FillPluginList(IEnumerable<IPlugin> plugins)
        {
            IEnumerable<UIPluginRepresentation> uiPlugins = plugins.Select(pl => new UIPluginRepresentation(pl));
            PluginsList = new ObservableCollection<UIPluginRepresentation>(uiPlugins);
            
            _autoResetEvent.Set();
        }

        private DelegateCommand _uninstallPluginCommand;
        public DelegateCommand UninstallPluginCommand =>
            _uninstallPluginCommand ?? (_uninstallPluginCommand = new DelegateCommand(ExecuteUninstallPluginCommand, () => SelectedPlugin != null)
            .ObservesProperty(() => SelectedPlugin));

        void ExecuteUninstallPluginCommand()
        {
            _ea.GetEvent<UninstallPluginEvent>().Publish(SelectedPlugin.GetPlugin());
            SelectedPlugin = null;
            DeleteAllPluginFiles = false;
        }
    }
}
