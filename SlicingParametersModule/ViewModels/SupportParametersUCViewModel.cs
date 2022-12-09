using LSlicer.Data.Model;
using LSlicer.Infrastructure;
using LSlicing.Data.Interaction.Contracts;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ParametersModule.ViewModels
{
    public class SupportParametersUCViewModel : BindableBase
    {
        private readonly IEventAggregator _ea;
        private ObservableCollection<ISupportParameters> _parameters;
        public ObservableCollection<ISupportParameters> Parameters
        {
            get { return _parameters; }
            set { SetProperty(ref _parameters, value); }
        }

        private void InitParameters(IEnumerable<ISupportParameters> supportParameters)
        {
            Parameters = new ObservableCollection<ISupportParameters>(supportParameters);
        }

        public SupportParametersUCViewModel(IEventAggregator ea)
        {
            _ea = ea;
            _ea.GetEvent<SupportParametersBatchSentEvent>().Subscribe(InitParameters);
        }

        private ISupportParameters _selectedParameters;
        public ISupportParameters SelectedParameters
        {
            get { return _selectedParameters; }
            set { SetProperty(ref _selectedParameters, value); }
        }

        private DelegateCommand _setParameters;
        public DelegateCommand SetParametersCommand =>
            _setParameters ?? (_setParameters = new DelegateCommand(ExecuteSetParametersCommand, CanExecuteCommand))
            .ObservesProperty(() => SelectedParameters);

        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand =>
            _saveCommand ?? (_saveCommand = new DelegateCommand(ExecuteSaveCommand, CanExecuteCommand))
            .ObservesProperty(() => SelectedParameters);

        private DelegateCommand _addCommand;
        public DelegateCommand AddCommand =>
            _addCommand ?? (_addCommand = new DelegateCommand(ExecuteAddCommand, () => Parameters != null))
            .ObservesProperty(() => SelectedParameters);

        private DelegateCommand _deleteCommand;
        public DelegateCommand DeleteCommand =>
            _deleteCommand ?? (_deleteCommand = new DelegateCommand(ExecuteDeleteCommand, CanExecuteRemoveCommand))
            .ObservesProperty(() => SelectedParameters);

        void ExecuteAddCommand()
        {
            ISupportParameters newParameters = _selectedParameters != null 
                ? (ISupportParameters)_selectedParameters.Clone()
                : new SupportParameters();
            newParameters.Id = Parameters.Max(x => x.Id) + 1;
            _parameters.Add(newParameters);
            _ea.GetEvent<SupportParametersAddEvent>().Publish(newParameters);
            RaisePropertyChanged(nameof(Parameters));
        }

        void ExecuteDeleteCommand()
        {
            if (Parameters.Count > 1)
            {
                _ea.GetEvent<SupportParametersDeleteEvent>().Publish(SelectedParameters);
                Parameters.Remove(SelectedParameters);
                RaisePropertyChanged(nameof(Parameters));
            }
        }

        void ExecuteSaveCommand()
        {
            _ea.GetEvent<SupportParametersSaveEvent>().Publish(Parameters.AsEnumerable());
        }

        void ExecuteSetParametersCommand()
        {
            _ea.GetEvent<SupportParametersSentEvent>().Publish(SelectedParameters);
        }

        bool CanExecuteCommand()
        {
            return Parameters != null && Parameters.Any();
        }

        bool CanExecuteRemoveCommand() => Parameters != null && SelectedParameters != null;
    }
}
