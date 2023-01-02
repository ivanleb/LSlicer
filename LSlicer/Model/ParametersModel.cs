using LSlicer.BL.Interaction;
using LSlicer.BL.Interaction.Contracts;
using LSlicer.Helpers;
using LSlicer.Infrastructure;
using LSlicer.Data.Interaction.Contracts;
using Prism.Events;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Unity;

namespace LSlicer.Model
{
    public class ParametersModel
    {
        private readonly IEventAggregator _ea;
        private readonly IUnityContainer _containerProvider;
        private readonly IAppSettings _appSettings;
        private readonly ILoggerService _logger;
        private readonly ISlicingParametersService _slicingParametersService;
        private readonly ISupportParametersService _supportParametersService;
        private readonly PresenterModel _presenterModel;
        private Window _parametersWindow;


        public ParametersModel(
            IEventAggregator ea,
            IUnityContainer containerProvider,
            IAppSettings appSettings,
            ILoggerService logger,
            ISlicingParametersService slicingParametersService,
            ISupportParametersService supportParametersService, 
            PresenterModel presenterModel)
        {
            _ea = ea;
            EventSubscribes();
            _containerProvider = containerProvider;
            _appSettings = appSettings;
            _logger = logger;
            _slicingParametersService = slicingParametersService;
            _supportParametersService = supportParametersService;
            _presenterModel = presenterModel;
            _presenterModel.GetSupportParametersIdentifiersDelegate = GetSupportParametersIdentifiers;
            _presenterModel.GetSlicingParametersIdentifiersDelegate = GetSlicingParametersIdentifiers;
        }

        #region Parameters
        public void RaiseSliceParametersView()
        {
            _parametersWindow = _containerProvider.Resolve<Views.SlicingParameters>();
            _parametersWindow.Owner = Application.Current.MainWindow;
            _parametersWindow.Show();

            _ea.GetEvent<SlicingParametersBatchSentEvent>()
                .Publish(_slicingParametersService.GetAll());
            _ea.GetEvent<SupportParametersBatchSentEvent>()
                .Publish(_supportParametersService.GetAll());
            _ea.GetEvent<SlicingParametersSaveEvent>()
                .Subscribe(SaveSlicingParameters);
            _ea.GetEvent<SupportParametersSaveEvent>()
                .Subscribe(SaveSupportParameters);

        }

        public void RaiseSupportParametersView()
        {
            _parametersWindow = _containerProvider.Resolve<Views.SupportParameters>();
            _parametersWindow.Owner = Application.Current.MainWindow;
            _parametersWindow.Show();

            _ea.GetEvent<SlicingParametersBatchSentEvent>()
                .Publish(_slicingParametersService.GetAll());
            _ea.GetEvent<SupportParametersBatchSentEvent>()
                .Publish(_supportParametersService.GetAll());
            _ea.GetEvent<SlicingParametersSaveEvent>()
                .Subscribe(SaveSlicingParameters);
            _ea.GetEvent<SupportParametersSaveEvent>()
                .Subscribe(SaveSupportParameters);
        }

        public IList<string> GetSupportParametersIdentifiers(int partId)
            => GetParametersIdentifiers(partId, _supportParametersService);

        public IList<string> GetSlicingParametersIdentifiers(int partId)
            => GetParametersIdentifiers(partId, _slicingParametersService);

        private void SetSlicingParameters(ISlicingParameters slicingParameters) =>
            SetParameters(slicingParameters, _slicingParametersService, "slice");

        private void SetSupportParameters(ISupportParameters supportParameters) =>
            SetParameters(supportParameters, _supportParametersService, "support");

        private void SetParameters<T>(T parameters, IParametersService<T> parametersService, string name)
        {
            string jobSpecFileName = FileNameResolver.UniqJobSpecNameGenerate(name, ".json");
            var specFile = new FileInfo(Path.Combine(PathHelper.Resolve(_appSettings.WorkingDirectory), jobSpecFileName));
            foreach (var partId in _presenterModel.SelectedPartIds)
            {
                parametersService.Set(parameters, partId, specFile);
            }
            _ea.GetEvent<ReloadPartInfoUIListEvent>().Publish();
        }

        private IList<string> GetParametersIdentifiers<T>(int partId, IParametersService<T> parametersService) 
            => parametersService.GetParametersIdentifiers(partId);

        private void AddSupportParameters(ISupportParameters supportParameters) =>
            _supportParametersService.Add(supportParameters);

        private void DeleteSupportParameters(ISupportParameters supportParameters) =>
            _supportParametersService.Delete(supportParameters);

        private void AddSlicingParameters(ISlicingParameters slicingParameters) =>
            _slicingParametersService.Add(slicingParameters);

        private void DeleteSlicingParameters(ISlicingParameters slicingParameters) =>
            _slicingParametersService.Delete(slicingParameters);

        private void SaveSlicingParameters(IEnumerable<ISlicingParameters> parameters) =>
            _slicingParametersService.Save(parameters);

        private void SaveSupportParameters(IEnumerable<ISupportParameters> parameters) =>
            _supportParametersService.Save(parameters);
        #endregion

        private void EventSubscribes()
        {
            _ea.GetEvent<SlicingParametersSentEvent>().Subscribe(SetSlicingParameters);
            _ea.GetEvent<SupportParametersSentEvent>().Subscribe(SetSupportParameters);
            _ea.GetEvent<SlicingParametersAddEvent>().Subscribe(AddSlicingParameters);
            _ea.GetEvent<SupportParametersAddEvent>().Subscribe(AddSupportParameters);
            _ea.GetEvent<SlicingParametersDeleteEvent>().Subscribe(DeleteSlicingParameters);
            _ea.GetEvent<SupportParametersDeleteEvent>().Subscribe(DeleteSupportParameters);
        }
    }
}
