using HelixToolkit.Wpf;
using LSlicer.BL.Events;
using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Contracts;
using LSlicer.BL.Interaction.Types;
using LSlicer.Data.Interaction;
using LSlicer.Extensions;
using LSlicer.Helpers;
using LSlicer.Infrastructure;
using LSlicer.Model;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace LSlicer.ViewModels
{
    public partial class ShellViewModel : BindableBase
    {
        private IEventAggregator _ea;
        private PresenterModel _presenterModel;
        private WorkTaskModel _workTaskModel;
        private SettingsController _settingsModel;
        private IdentityController _identityModel;
        private ParametersModel _parametersModel;
        private PluginController _pluginModel;
        private ILoggerService _logger;
        private StatusUploader _statusUploader;
        private IList<Model3D> _selectedModels = new List<Model3D>();
        private HelixViewport3D _viewPort;
        private IValidator<string> _3dFileValidator;

        private MouseBinding _rectangleSelectionMouseBinding;
        private MouseBinding _pointSelectionMouseBinding;
        private MouseBinding _startPointStraightedgeSelectionMouseBinding;

        #region BINDABLE VARIABLES SECTION
        public ObservableCollection<Visual3D> Objects { get; set; }
        public ObservableCollection<UIPartInfo> Parts { get; set; } = new ObservableCollection<UIPartInfo>();
        public ObservableCollection<UIPartInfo> SelectedParts
        {
            get { return _presenterModel.SelectedParts; }
            set { SetProperty(ref _presenterModel.SelectedParts, value); }
        }

        private bool _autoSaveEnabled;
        public bool AutoSaveEnabled
        {
            get => _autoSaveEnabled;
            set => SetProperty(ref _autoSaveEnabled, value);
        }

        private bool _needAddManipulator = false;
        public bool NeedAddManipulator
        {
            get { return _needAddManipulator; }
            set { _needAddManipulator = value; }
        }

        private string _selectedSupportEngine;
        public string SelectedSupportEngine
        {
            get
            {
                if (_selectedSupportEngine != _settingsModel.SelectedSupportEngine) _selectedSupportEngine = _settingsModel.SelectedSupportEngine;
                return _settingsModel.SelectedSupportEngine;
            }
            set
            {
                SetProperty(ref _selectedSupportEngine, value);
                _settingsModel.SelectedSupportEngine = value;
            }
        }

        private ObservableCollection<string> _supportEngineList;
        public ObservableCollection<string> SupportEngineList
        {
            get { return _supportEngineList; }
            set { SetProperty(ref _supportEngineList, value); }
        }

        private string _selectedSliceEngine;
        public string SelectedSliceEngine
        {
            get
            {
                if (_selectedSliceEngine != _settingsModel.SelectedSliceEngine) _selectedSliceEngine = _settingsModel.SelectedSliceEngine;
                return _settingsModel.SelectedSliceEngine;
            }
            set
            {
                SetProperty(ref _selectedSliceEngine, value);
                _settingsModel.SelectedSliceEngine = value;
            }
        }

        private ObservableCollection<string> _sliceEngineList;
        public ObservableCollection<string> SliceEngineList
        {
            get { return _sliceEngineList; }
            set { SetProperty(ref _sliceEngineList, value); }
        }

        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                SetProperty(ref _status, value);
                _logger.Info($"Status: {StatusUploader.ExceptProgressFilter(_status)}");
            }
        }

        private bool _drawRuler;
        public bool DrawRuler
        {
            get
            {
                return _drawRuler;
            }
            set
            {
                _drawRuler = value;
                ChangeRuleDrawRuler();
            }
        }

        private double _straightedgeLength = 1;
        public double StraightedgeLength
        {
            get { return _straightedgeLength; }
            set { SetProperty(ref _straightedgeLength, value); }
        }

        private bool _drawStraightedge;
        public bool DrawStraightedge
        {
            get
            {
                return _drawStraightedge;
            }
            set
            {
                _drawStraightedge = value;
                ChangeRuleDrawStraightedge();
            }
        }

        public Point3D? CursorOnElementPosition
        {
            set
            {
                if (value.HasValue && _needAddManipulator)
                    _ea.GetEvent<AbsoluteCoordinateSentEvent>().Publish((Vector3D)value.Value);
            }
        }
        private Visibility _cursorPositionCardVisibility = Visibility.Hidden;
        public Visibility CursorPositionCardVisibility
        {
            get { return _cursorPositionCardVisibility; }
            set { SetProperty(ref _cursorPositionCardVisibility, value); }
        }

        private string _resultName = "default name";
        public string ResultName
        {
            get { return _resultName; }
            set { SetProperty(ref _resultName, value); }
        }
        #endregion BINDABLE VARIABLES SECTION

        public ShellViewModel(
            PresenterModel presenterModel,
            WorkTaskModel workTaskModel,
            ILoggerService loggerService,
            IEventAggregator ea,
            IdentityController identityModel,
            SettingsController settingsModel,
            ParametersModel parametersModel, 
            PluginController pluginModel,
            IValidator<string> threeDFileValidator)
        {
            _ea = ea;
            _presenterModel = presenterModel;
            _workTaskModel = workTaskModel;
            _logger = loggerService;
            _identityModel = identityModel;
            _settingsModel = settingsModel;
            _status = "start";
            _statusUploader = new StatusUploader((s) => Status = s);
            _presenterModel.SubscribeMessageStream(_statusUploader);
            Objects = new ObservableCollection<Visual3D>();
            SupportEngineList = new ObservableCollection<string>(_settingsModel.GetSupportEngineList());
            SliceEngineList = new ObservableCollection<string>(_settingsModel.GetSliceEngineList());
            _presenterModel.DisplayObjects = Objects;
            _ea.GetEvent<TransformSentEvent>().Subscribe(TransformPart);
            _parametersModel = parametersModel;
            SelectedParts.CollectionChanged += (s, e) => ChangingSelectedPartsCountHandler();
            _3dFileValidator = threeDFileValidator;
            _ea.GetEvent<ReloadPartInfoUIListEvent>().Subscribe(ReloadPartInfos);
            _pluginModel = pluginModel;
        }

        #region ACTION SECTION
        private void ClearCommandAction()
        {
            _presenterModel.GetAllDisplayObjects().ForEach(obj => Objects.Remove(obj));
            SelectedParts.Clear();
            Parts.Clear();
            _presenterModel.Parts.ToArray().ForEach(part => _presenterModel.UnloadPart(part.Id));
        }

        public void ReloadPartInfos()
        {
            Parts.Clear();
            _presenterModel.GetPartInfos()
                .ToList()
                .ForEach(pi => Parts.Add(pi));
        }

        private void ExecuteShowRulerAction()
        {
            CursorPositionCardVisibility =
                CursorPositionCardVisibility == Visibility.Visible
                    ? Visibility.Hidden
                    : Visibility.Visible;

            DrawRuler = false;
        }

        private void CloseApplicationAction()
        {
            AutoSaveEnabled = false;
            ActionHelper.SafeExecutionWithQuestion("Save changes?", SaveWorkStateAction);
            Application.Current.Shutdown();
        }

        private void UndoAction()
        {
            int[] partIds = SelectedParts.Select(p => int.Parse(p.Id)).ToArray();
            _presenterModel.UndoAction(partIds);
        }
        #endregion ACTION SECTION
        #region COMMAND SECTION
        private DelegateCommand _saveCurrentChangesCommand;
        public DelegateCommand SaveCurrentChangesCommand =>
            _saveCurrentChangesCommand ?? (_saveCurrentChangesCommand = new DelegateCommand(SaveWorkStateAction));

        private DelegateCommand _openCurrentChangesCommand;
        public DelegateCommand OpenCurrentChangesCommand =>
            _openCurrentChangesCommand ?? (_openCurrentChangesCommand = new DelegateCommand(OpenWorkStateAction));

        private DelegateCommand _autoSaveEnableCommand;
        public DelegateCommand AutoSaveEnableCommand =>
            _autoSaveEnableCommand ?? (_autoSaveEnableCommand = new DelegateCommand(SaveWorkStateAction));

        private ShowExceptionDelegateCommand _saveResultCommand;
        public ShowExceptionDelegateCommand SaveResultCommand =>
            _saveResultCommand ?? (_saveResultCommand = new ShowExceptionDelegateCommand(() => _presenterModel.SaveResult(ResultName)) { RepeatCount = 3 });

        private DelegateCommand _openFileCommand;
        public DelegateCommand OpenFileCommand =>
            _openFileCommand ?? (_openFileCommand = new DelegateCommand(OpenFileAction));

        private DelegateCommand _closeApplicationCommand;
        public DelegateCommand CloseApplicationCommand =>
            _closeApplicationCommand ?? (_closeApplicationCommand = new DelegateCommand(CloseApplicationAction));

        private DelegateCommand _sliceAllCommand;
        public DelegateCommand SliceAllCommand =>
            _sliceAllCommand ?? (_sliceAllCommand = new DelegateCommand(() => _workTaskModel.SliceAll()));

        private DelegateCommand _supportAllCommand;
        public DelegateCommand SupportAllCommand =>
            _supportAllCommand ?? (_supportAllCommand = new DelegateCommand(() => _workTaskModel.MakeSupportsForAll()));

        private DelegateCommand _openSliceParametersCommand;
        public DelegateCommand OpenSliceParametersCommand =>
            _openSliceParametersCommand ?? (_openSliceParametersCommand = new DelegateCommand(() => _parametersModel.RaiseSliceParametersView()));

        private DelegateCommand _openSupportParametersCommand;
        public DelegateCommand OpenSupportParametersCommand =>
            _openSupportParametersCommand ?? (_openSupportParametersCommand = new DelegateCommand(() => _parametersModel.RaiseSupportParametersView()));

        private DelegateCommand _openDirectorySettingsCommand;
        public DelegateCommand OpenDirectorySettingsCommand =>
            _openDirectorySettingsCommand ?? (_openDirectorySettingsCommand = new DelegateCommand(() => _settingsModel.RaiseSettingsView()));

        private DelegateCommand _openColorSettingsCommand;
        public DelegateCommand OpenColorSettingsCommand =>
            _openColorSettingsCommand ?? (_openColorSettingsCommand = new DelegateCommand(() => _settingsModel.RaiseColorSettingsView()));

        private DelegateCommand _openAdditionSettingsCommand;
        public DelegateCommand OpenAdditionSettingsCommand =>
            _openAdditionSettingsCommand ?? (_openAdditionSettingsCommand = new DelegateCommand(() => _settingsModel.RaiseAdditionSettingsView()));

        private DelegateCommand _openLoginCommand;
        public DelegateCommand OpenLoginCommand =>
            _openLoginCommand ?? (_openLoginCommand = new DelegateCommand(() => _identityModel.RaiseLoginView()));

        private DelegateCommand _undoLastActionCommand;
        public DelegateCommand UndoLastActionCommand =>
            _undoLastActionCommand ?? (_undoLastActionCommand = new DelegateCommand(UndoAction));

        private DelegateCommand _updateRulerCommand;
        public DelegateCommand UpdateRulerCommand =>
            _updateRulerCommand ?? (_updateRulerCommand = new DelegateCommand(() => UpdateRulerAction()));

        private DelegateCommand _updateStraightedgeCommand;
        public DelegateCommand UpdateStraightedgeCommand =>
            _updateStraightedgeCommand ?? (_updateStraightedgeCommand = new DelegateCommand(() => UpdateStraightedgeAction()));

        private DelegateCommand _addStartPointStraightedgeCommand;
        public DelegateCommand AddStartPointStraightedgeCommand =>
            _addStartPointStraightedgeCommand ?? (_addStartPointStraightedgeCommand = new DelegateCommand(() => AddStartPointStraightedgeAction()));

        private DelegateCommand _removePartCommand;
        public DelegateCommand RemovePartCommand =>
            _removePartCommand ?? (_removePartCommand = new DelegateCommand(UnloadPartAction, () => SelectedParts.Any())
            .ObservesProperty(() => SelectedParts.Count));

        private DelegateCommand _copyPartCommand;
        public DelegateCommand CopyPartCommand =>
            _copyPartCommand ?? (_copyPartCommand = new DelegateCommand(CopyPartAction, () => SelectedParts.Any())
            .ObservesProperty(() => SelectedParts.Count));

        private DelegateCommand _cancelCammand;
        public DelegateCommand CancelCammand =>
            _cancelCammand ?? (_cancelCammand = new DelegateCommand(() => _workTaskModel.CancelAllTasks()));

        private DelegateCommand _showRulerCommand;
        public DelegateCommand ShowRulerCommand =>
            _showRulerCommand ?? (_showRulerCommand = new DelegateCommand(ExecuteShowRulerAction));
        
        private DelegateCommand _showPluginCommanderCommand;
        public DelegateCommand ShowPluginCommanderCommand =>
            _showPluginCommanderCommand ?? (_showPluginCommanderCommand = new DelegateCommand(() => _pluginModel.RaisePluginWindow()));

        public RectangleSelectionCommand RectangleSelectionCommand { get; private set; }
        public PointSelectionCommand PointSelectionCommand { get; private set; }
        public PointSelectionCommand StartPointStraightedgeCommand { get; private set; }

        private ShowExceptionDelegateCommand _viewLogsCommand;
        public ShowExceptionDelegateCommand ViewLogsCommand =>
            _viewLogsCommand ?? (_viewLogsCommand = new ShowExceptionDelegateCommand(() => Process.Start(PathHelper.Resolve("{APPDATA}\\LSlicer\\Logs\\"))));

        private DelegateCommand _renewCommand;
        public DelegateCommand RenewCommand =>
            _renewCommand ?? (_renewCommand = new DelegateCommand(ReloadPartInfos));

        private ShowExceptionDelegateCommand _clearCommand;
        public ShowExceptionDelegateCommand ClearCommand =>
            _clearCommand ?? (_clearCommand = new ShowExceptionDelegateCommand(ClearCommandAction));

        public PresenterModel PresenterModel => _presenterModel;
        public WorkTaskModel WorkTaskModel => _workTaskModel;
        #endregion COMMAND SECTION

    }
}