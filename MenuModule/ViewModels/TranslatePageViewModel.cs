using LSlicer.Data.Model;
using LSlicer.Infrastructure;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.Windows.Media.Media3D;

namespace MenuModule.ViewModels
{
    public class TranslatePageViewModel : BindableBase
    {
        private string _units = "mm";
        public string Units
        {
            get { return _units; }
            set { SetProperty(ref _units, value); }
        }

        private string _translateUnits = "mm";
        public string TranslateUnits
        {
            get { return _translateUnits; }
            set { SetProperty(ref _translateUnits, value); }
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

        private double _relativeX;
        public double RelativeX
        {
            get { return _relativeX; }
            set { SetProperty(ref _relativeX, value); }
        }

        private double _relativeY;
        public double RelativeY
        {
            get { return _relativeY; }
            set { SetProperty(ref _relativeY, value); }
        }

        private double _relativeZ;
        public double RelativeZ
        {
            get { return _relativeZ; }
            set { SetProperty(ref _relativeZ, value); }
        }


        private DelegateCommand _applyTranslateCommand;
        public DelegateCommand ApplyTranslateCommand =>
            _applyTranslateCommand ?? (_applyTranslateCommand = new DelegateCommand(ExecuteRelativeTranslateCommand)
                                                .ObservesProperty(() => RelativeX)
                                                .ObservesProperty(() => RelativeY)
                                                .ObservesProperty(() => RelativeZ));

        /*
        private DelegateCommand _applyRelativeTranslateCommand;
        public DelegateCommand ApplyRelativeTranslateCommand =>
            _applyRelativeTranslateCommand ?? (_applyRelativeTranslateCommand = new DelegateCommand(ExecuteRelativeTranslateCommand)
                                                .ObservesProperty(() => AbsoluteX)
                                                .ObservesProperty(() => AbsoluteY)
                                                .ObservesProperty(() => AbsoluteZ)
                                                .ObservesProperty(() => RelativeX)
                                                .ObservesProperty(() => RelativeY)
                                                .ObservesProperty(() => RelativeZ));

        private DelegateCommand _applyAbsoluteTranslateCommand;
        public DelegateCommand ApplyAbsoluteTranslateCommand =>
            _applyAbsoluteTranslateCommand ?? (_applyAbsoluteTranslateCommand = new DelegateCommand(ExecuteAbsoluteTranslateCommand)
                                                .ObservesProperty(() => AbsoluteX)
                                                .ObservesProperty(() => AbsoluteY)
                                                .ObservesProperty(() => AbsoluteZ)
                                                .ObservesProperty(() => RelativeX)
                                                .ObservesProperty(() => RelativeY)
                                                .ObservesProperty(() => RelativeZ));
        */

        void ExecuteRelativeTranslateCommand()
        {
            /*
            _resultTransform.Translate(new Vector3D(RelativeX, RelativeY, RelativeZ));
            //_ea.GetEvent<TransformSentEvent>().Publish(new PartTransform($"Relative Translate x:{RelativeX}, y:{RelativeY}, z:{RelativeZ}", _resultTransform, new Matrix3D()));
            //_ea.GetEvent<TransformSentEvent>().Publish(new PartTransform($"Relative Translate x:{RelativeX}, y:{RelativeY}, z:{RelativeZ}", new Vector3D(RelativeX, RelativeY, RelativeZ), new Matrix3D()));
            //_resultTransform.Translate(new Vector3D(-RelativeX, -RelativeY, -RelativeZ));
            _resultTransform = Matrix3D.Identity;
            */
            //затычка

            //_resultTransform.Translate(new Quaternion(new Vector3D(1, 0, 0), RelativeX));
            //_resultTransform.Translate(new Quaternion(new Vector3D(0, 1, 0), RelativeY));
            //_resultTransform.Translate(new Quaternion(new Vector3D(0, 0, 1), RelativeZ));
            _resultTransform.Translate(new Vector3D(RelativeX, RelativeY, RelativeZ));
            _ea.GetEvent<TransformSentEvent>().Publish(new PartTransform($"Relative translate x:{RelativeX}, y:{RelativeY}, z:{RelativeZ}", _resultTransform, new Matrix3D()));
            _resultTransform.M11 = 1;
            _resultTransform.M12 = 0;
            _resultTransform.M13 = 0;
            _resultTransform.M21 = 0;
            _resultTransform.M22 = 1;
            _resultTransform.M23 = 0;
            _resultTransform.M31 = 0;
            _resultTransform.M32 = 0;
            _resultTransform.M33 = 1;
            _resultTransform.OffsetX = 0;
            _resultTransform.OffsetY = 0;
            _resultTransform.OffsetZ = 0;

        }

        void ExecuteAbsoluteTranslateCommand()
        {

            //_resultTransform.Translate(new Vector3D(AbsoluteX, AbsoluteY, AbsoluteZ));
            //_ea.GetEvent<TransformSentEvent>().Publish(new PartTransform($"Absolute Translate x:{AbsoluteX}, y:{AbsoluteY}, z:{AbsoluteZ}", _resultTransform, new Matrix3D()));
            //_ea.GetEvent<TransformSentEvent>().Publish(new PartTransform($"Absolute Translate x:{AbsoluteX}, y:{AbsoluteY}, z:{AbsoluteZ}", new Vector3D(AbsoluteX, AbsoluteY, AbsoluteZ), new Matrix3D()));
            _resultTransform = Matrix3D.Identity;
            
        }



        private readonly IEventAggregator _ea;

       

        public TranslatePageViewModel(IEventAggregator ea)
        {
            _ea = ea;
            _ea.GetEvent<AbsoluteCoordinateSentEvent>().Subscribe(GetRelativeCoordinate);
        }

        private Matrix3D _resultTransform;

        private void GetRelativeCoordinate(Vector3D translation) 
        {
            //AbsoluteX = translation.X;
            //AbsoluteY = translation.Y;
            //AbsoluteZ = translation.Z;
        }
    }

}
