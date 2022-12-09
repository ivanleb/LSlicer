using LSlicer.BL.Interaction;
using LSlicer.Model;
using PluginFramework;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LSlicer.ViewModels
{
    //public class PluginControlPanelViewModel : BindableBase
    //{

    //    private readonly Lazy<IEnumerable<IPlugin>> _pluginsContainer = new Lazy<IEnumerable<IPlugin>>(() => GetPlugins());



    //    public PluginControlPanelViewModel(ILoggerService logger)
    //    {
    //        _logger = logger;
    //    }

    //    private UIPluginRepresentation _selectedPlugin;
    //    public UIPluginRepresentation SelectedPlugin
    //    {
    //        get { return _selectedPlugin; }
    //        set { SetProperty(ref _selectedPlugin, value); }
    //    }

    //    public ReadOnlyObservableCollection<UIPluginRepresentation> _pluginsList;
    //    public ReadOnlyObservableCollection<UIPluginRepresentation> PluginsList
    //    {
    //        get
    //        {
    //            return _pluginsList ?? (_pluginsList = InitializePluginCollection());
    //        }
    //        set { }
    //    }



    //    private DelegateCommand _runPluginCommand;
    //    public DelegateCommand RunPluginCommand =>
    //        _runPluginCommand ?? (_runPluginCommand = new DelegateCommand(() => SelectedPlugin.RunPlugin(), () => SelectedPlugin != null)
    //        .ObservesProperty(() => SelectedPlugin));

    //}
}
