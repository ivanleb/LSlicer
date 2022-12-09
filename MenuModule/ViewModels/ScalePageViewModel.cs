using LSlicer.Data.Model;
using LSlicer.Infrastructure;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace MenuModule.ViewModels
{
    public class ScalePageViewModel : BindableBase
    {
        private string _units = "mm";
        public string Units
        {
            get { return _units; }
            set { SetProperty(ref _units, value); }
        }

        private double _scaleX = 1;
        public double ScaleX
        {
            get { return _scaleX; }
            set { SetProperty(ref _scaleX, value); }
        }

        private double _scaleY = 1;
        public double ScaleY
        {
            get { return _scaleY; }
            set { SetProperty(ref _scaleY, value); }
        }

        private double _scaleZ = 1;
        public double ScaleZ
        {
            get { return _scaleZ; }
            set { SetProperty(ref _scaleZ, value); }
        }

        private DelegateCommand _applyScaleCommand;
        public DelegateCommand ApplyScaleCommand =>
            _applyScaleCommand ?? (_applyScaleCommand = new DelegateCommand(ExecuteScaleCommand)
                                                .ObservesProperty(() => ScaleX)
                                                .ObservesProperty(() => ScaleY)
                                                .ObservesProperty(() => ScaleZ));

        void ExecuteScaleCommand()
        {
            _resultTransform.Scale(new Vector3D(_scaleX, _scaleY, _scaleZ));
            //_resultTransform.Scale(new Vector3D(0.5, 0.5, 0.5));
            _ea.GetEvent<TransformSentEvent>().Publish(new PartTransform($"ScaleX: {_scaleX}, ScaleY: { _scaleY}, ScaleZ: {_scaleZ}", _resultTransform, new Matrix3D()));
            //_resultTransform.Scale(new Vector3D(-_scale, -_scale, -_scale));
            //_ea.GetEvent<TransformSentEvent>().Publish(_resultTransform);
            _resultTransform.M11 = 1;
            _resultTransform.M12 = 0;
            _resultTransform.M13 = 0;
            _resultTransform.M21 = 0;
            _resultTransform.M22 = 1;
            _resultTransform.M23 = 0;
            _resultTransform.M31 = 0;
            _resultTransform.M32 = 0;
            _resultTransform.M33 = 1;
            _resultTransform.M44 = 1;

        }

        private readonly IEventAggregator _ea;



        public ScalePageViewModel(IEventAggregator ea)
        {
            _ea = ea;
        }

        private Matrix3D _resultTransform;
    }
}
