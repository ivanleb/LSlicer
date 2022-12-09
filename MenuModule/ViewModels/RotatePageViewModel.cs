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
    public class RotatePageViewModel : BindableBase
    {
        private string _units = "mm";
        public string Units
        {
            get { return _units; }
            set { SetProperty(ref _units, value); }
        }

        private double _absoluteX;
        public double AbsoluteX
        {
            get { return _absoluteX; }
            set { SetProperty(ref _absoluteX, value); }
        }

        private double _absoluteY;
        public double AbsoluteY
        {
            get { return _absoluteY; }
            set { SetProperty(ref _absoluteY, value); }
        }

        private double _absoluteZ;
        public double AbsoluteZ
        {
            get { return _absoluteZ; }
            set { SetProperty(ref _absoluteZ, value); }
        }

        private double _rotateX;
        public double RotateX
        {
            get { return _rotateX; }
            set { SetProperty(ref _rotateX, value); }
        }

        private double _rotateY;
        public double RotateY
        {
            get { return _rotateY; }
            set { SetProperty(ref _rotateY, value); }
        }

        private double _rotateZ;
        public double RotateZ
        {
            get { return _rotateZ; }
            set { SetProperty(ref _rotateZ, value); }
        }

        private double _rotateSnap;
        public double RotateSnap
        {
            get { return _rotateSnap; }
            set { SetProperty(ref _rotateSnap, value); }
        }

        private string _rotateUnits = "grad";
        public string RotateUnits
        {
            get { return _rotateUnits; }
            set { SetProperty(ref _rotateUnits, value); }
        }

        private DelegateCommand _applyRotateCommand;
        public DelegateCommand ApplyRotateCommand =>
            _applyRotateCommand ?? (_applyRotateCommand = new DelegateCommand(ExecuteRotateCommand)
                                                .ObservesProperty(() => RotateX)
                                                .ObservesProperty(() => RotateY)
                                                .ObservesProperty(() => RotateZ));

        void ExecuteRotateCommand()
        {
            _resultTransform.Rotate(new Quaternion(new Vector3D(1, 0, 0), RotateX));
            _resultTransform.Rotate(new Quaternion(new Vector3D(0, 1, 0), RotateY));
            _resultTransform.Rotate(new Quaternion(new Vector3D(0, 0, 1), RotateZ));
            _ea.GetEvent<TransformSentEvent>().Publish(new PartTransform($"Rotate x:{RotateX}, y:{RotateY}, z:{RotateZ}", _resultTransform, new Matrix3D()));
            _resultTransform.M11 = 1;
            _resultTransform.M12 = 0;
            _resultTransform.M13 = 0;
            _resultTransform.M21 = 0;
            _resultTransform.M22 = 1;
            _resultTransform.M23 = 0;
            _resultTransform.M31 = 0;
            _resultTransform.M32 = 0;
            _resultTransform.M33 = 1;
        }

        
        private readonly IEventAggregator _ea;



        public RotatePageViewModel(IEventAggregator ea)
        {
            _ea = ea;
        }

        private Matrix3D _resultTransform;
    }
}
