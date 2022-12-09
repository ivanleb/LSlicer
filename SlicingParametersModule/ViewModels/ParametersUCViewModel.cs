using LSlicer.Infrastructure;
using LSlicing.Data.Interaction.Contracts;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParametersModule.ViewModels
{
    public class ParametersUCViewModel : BindableBase
    {
        private readonly IEventAggregator _ea;
        private ObservableCollection<ISlicingParameters> _parameters;
        public ObservableCollection<ISlicingParameters> Parameters
        {
            get { return _parameters; }
            set { SetProperty(ref _parameters, value); }
        }

        private void InitParameters(IEnumerable<ISlicingParameters> slicingParameters) 
        {
            Parameters = new ObservableCollection<ISlicingParameters>(slicingParameters);
        }

        public ParametersUCViewModel(IEventAggregator ea)
        {
            _ea = ea;
            _ea.GetEvent<SlicingParametersBatchSentEvent>().Subscribe(InitParameters);
        }

        private ISlicingParameters _selectedParameters;
        public ISlicingParameters SelectedParameters
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
            _addCommand ?? (_addCommand = new DelegateCommand(ExecuteAddCommand, CanExecuteCommand))
            .ObservesProperty(() => SelectedParameters);

        private DelegateCommand _deleteCommand;
        public DelegateCommand DeleteCommand =>
            _deleteCommand ?? (_deleteCommand = new DelegateCommand(ExecuteDeleteCommand, CanExecuteCommand))
            .ObservesProperty(() => SelectedParameters);

        void ExecuteSaveCommand()
        {
            _ea.GetEvent<SlicingParametersSaveEvent>().Publish(Parameters.AsEnumerable());
        }

        void ExecuteSetParametersCommand()
        {
            _ea.GetEvent<SlicingParametersSentEvent>().Publish(_selectedParameters);
        }

        void ExecuteAddCommand()
        {         
            var newParameters = new LSlicer.Data.Model.SlicingParameters();
            newParameters.Thickness = _selectedParameters.Thickness;
            newParameters.Id = 0;
            for (var i = 0; i <  Parameters.Count; i++)
            {
                if(newParameters.Id <= Parameters[i].Id)
                {
                    newParameters.Id = Parameters[i].Id + 1;
                }
            }
            _parameters.Add(newParameters);
            _ea.GetEvent<SlicingParametersAddEvent>().Publish(newParameters);
            RaisePropertyChanged("Parameters");
        }

        void ExecuteDeleteCommand()
        {
            if (Parameters.Count > 1)
            {
                var newParameters = new LSlicer.Data.Model.SlicingParameters();
                newParameters.Thickness = _selectedParameters.Thickness;
                newParameters.Id = _selectedParameters.Id;

                Parameters.Remove(SelectedParameters);
                _ea.GetEvent<SlicingParametersDeleteEvent>().Publish(newParameters);
                RaisePropertyChanged("Parameters");
            }
        }

        bool CanExecuteCommand()
        {
            return Parameters !=null && Parameters.Any();
        }
    }
}
