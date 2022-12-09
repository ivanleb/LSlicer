using HelixToolkit.Wpf;
using LSlicer.Helpers;
using LSlicer.Model;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace LSlicer.ViewModels
{
    public partial class ShellViewModel : BindableBase
    {
        #region SELECTION PART

        public void SetViewPortCommands(HelixViewport3D viewport)
        {

            RectangleSelectionCommand = new RectangleSelectionCommand(viewport.Viewport, (s, e) => { }, HandleSelectionVisualsEvent);
            PointSelectionCommand = new PointSelectionCommand(viewport.Viewport, (s, e) => { }, HandleSelectionVisualsEvent);
            _viewPort = viewport;
            _viewPort.CalculateCursorPosition = true;
            Platform.Create100().ToList().ForEach(element => _viewPort.Children.Add(element));

            _rectangleSelectionMouseBinding = new MouseBinding(RectangleSelectionCommand, new MouseGesture(MouseAction.LeftClick));
            _pointSelectionMouseBinding = new MouseBinding(PointSelectionCommand, new MouseGesture(MouseAction.LeftClick, ModifierKeys.Control));
            
            SetSelectionMouseBindings();
        }

        public void SetSelectionMouseBindings()
        {
            if (!_viewPort.InputBindings.Contains(_rectangleSelectionMouseBinding))
                _viewPort.InputBindings.Add(_rectangleSelectionMouseBinding);

            if (!_viewPort.InputBindings.Contains(_pointSelectionMouseBinding))
                _viewPort.InputBindings.Add(_pointSelectionMouseBinding);
        }

        public void UnSetSelectionMouseBindings()
        {
            if(_viewPort.InputBindings.Contains(_rectangleSelectionMouseBinding))
                _viewPort.InputBindings.Remove(_rectangleSelectionMouseBinding);

            if (_viewPort.InputBindings.Contains(_pointSelectionMouseBinding))
                _viewPort.InputBindings.Remove(_pointSelectionMouseBinding);
        }

        

        private void HandleSelectionVisualsEvent(object sender, VisualsSelectedEventArgs e)
        {
            SelectedParts.Clear();
            foreach (var selectedVisual in e.SelectedVisuals)
            {
                _presenterModel.GetDisplayObjectId(selectedVisual)
                    .Match(
                        id =>
                        {
                            var part = Parts.FirstOrDefault(x => int.Parse(x.Id) == id);
                            if (part != null) SelectedParts.Add(part);
                        },
                        () => { }
                    );

            }
        }

        private void ChangeMaterial(IEnumerable<Model3D> models, Material material)
        {
            foreach (var model in models)
            {
                if (model is GeometryModel3D geometryModel3D)

                    geometryModel3D.Material = geometryModel3D.BackMaterial = material;
            }
        }

        private void ChangeMaterial(IEnumerable<Model3D> models, Material body, Material support)
        {
        }

        private void ChangingSelectedPartsCountHandler()
        {
            ChangeMaterial(_selectedModels, Materials.Blue);
            if (SelectedParts.Any())
            {
                _presenterModel.SelectedPartIds = SelectedParts.Select(x => int.Parse(x.Id)).ToList();

                RemoveManipulators();

                _selectedModels.Clear();
                foreach (var part in SelectedParts)
                {
                    _presenterModel.GetDisplayObjectById(int.Parse(part.Id))
                        .Match(visual => AddSelectedModel(visual), () => { });

                    //_presenterModel.GetDisplayObjectById(int.Parse(part.Id))
                    //    .Match(visual => AddRotateMultiplier(visual), () => { });

                    if (_needAddManipulator)
                    {
                        _presenterModel.GetDisplayObjectById(int.Parse(part.Id))
                            .Match(visual => AddManipulators(visual), () => { });
                    }

                }


                ChangeMaterial(_selectedModels, Materials.Orange);
            }
        }

        private void AddSelectedModel(Visual3D visual)
        {
            if (visual.GetPrivateProperty<Model3D>("Visual3DModel") is Model3DGroup model3DGroup)
            {
                Model3D model = model3DGroup.Children.FirstOrDefault();
                if (model != null) _selectedModels.Add(model);
            }
        }

        #endregion SELECTION PART

        #region MANIPULATION PART
        private void RemoveManipulators()
        {
            _viewPort.Children
                .Where(e => e is CombinedManipulator)
                .ToList()
                .ForEach(e => _viewPort.Children.Remove(e));
        }

        private void AddManipulators(Visual3D visual)
        {
            Rect3D bounds = visual.FindBounds(visual.Transform);

            CombinedManipulator combinedManipulator = new CombinedManipulator();

            var model3dVisual = visual as ModelVisual3D;
            if (model3dVisual == null) return;

            combinedManipulator.Bind(model3dVisual);

            foreach (var manipulator in combinedManipulator.Children)
            {
                switch (manipulator)
                {
                    case TranslateManipulator translateManipulator:
                        translateManipulator.Length = Math.Max(bounds.SizeX, Math.Max(bounds.SizeY, bounds.SizeZ));
                        translateManipulator.Diameter = 1;
                        break;

                    case RotateManipulator rotateManipulator:
                        rotateManipulator.Diameter = Math.Max(bounds.SizeX, Math.Max(bounds.SizeY, bounds.SizeZ)) * 1.5;
                        break;

                    default:
                        break;
                }
            }

            _viewPort.Children.Add(combinedManipulator);
        }

        private void AddRotateMultiplier(Visual3D visual)
        {

        }
        #endregion MANIPULATION PART

        #region RULER 
        private void ChangeRuleDrawRuler()
        {
            if (_drawRuler && _viewPort.CalculateCursorPosition)
                AddRuler();
            else
                DeleteRuler();
        }

        private void ChangeRuleDrawStraightedge()
        {
            if (_drawStraightedge && _viewPort.CalculateCursorPosition)
                AddStraightedge();
            else
                DeleteStraightedge();
        }

        private void AddRuler()
        {
            _viewPort.Children.Add(new RulerModel());
            _viewPort.Children.Add(new RulerNotchModel());
        }

        private void AddStraightedge()
        {
            _viewPort.Children.Add(new HelixToolkit.Wpf.ArrowVisual3D());
            //_viewPort.Children.Add(new StraightedgeModel());
            //_viewPort.Children.Add(new StraightedgeNotchModel());
        }

        private void UpdateRulerAction()
        {
            if (_viewPort == null || _viewPort.Children == null) return;

            foreach (var child in _viewPort.Children)
            {
                switch (child)
                {
                    case RulerModel rulerModel:
                        if (_viewPort.CursorOnElementPosition.HasValue)
                            rulerModel.UpdateModel(Point.Origin(), _viewPort.CursorOnElementPosition.Value);
                        else
                            rulerModel.UpdateModel(Point.Origin(), Point.Origin());
                        break;

                    case RulerNotchModel rulerNotchModel:
                        if (_viewPort.CursorOnElementPosition.HasValue)
                            rulerNotchModel.UpdateModel(Point.Origin(), _viewPort.CursorOnElementPosition.Value);
                        else
                            rulerNotchModel.UpdateModel(Point.Origin(), Point.Origin());
                        break;

                    default:
                        break;
                }
            }
        }

        private void UpdateStraightedgeAction()
        {
            if (_viewPort == null || _viewPort.Children == null) return;

            foreach (var child in _viewPort.Children)
            {
                switch (child)
                {
                    /*
                    case StraightedgeModel rulerModel:
                        if (_viewPort.CursorOnElementPosition.HasValue)
                        {
                            rulerModel.UpdateModel(Point.Origin(), _viewPort.CursorOnElementPosition.Value);
                            StraightedgeLength = rulerModel.Length;
                        }
                        else
                        {
                            rulerModel.UpdateModel(Point.Origin(), Point.Origin());
                            StraightedgeLength = rulerModel.Length;
                        }
                        break;

                    case StraightedgeNotchModel rulerNotchModel:
                        if (_viewPort.CursorOnElementPosition.HasValue)
                            rulerNotchModel.UpdateModel(Point.Origin(), _viewPort.CursorOnElementPosition.Value);
                        else
                            rulerNotchModel.UpdateModel(Point.Origin(), Point.Origin());
                        break;
                    */
                    case ArrowVisual3D rulerModel:
                        if (_viewPort.CursorOnElementPosition.HasValue)
                        {
                            rulerModel.Point2 = _viewPort.CursorOnElementPosition.Value;
                            StraightedgeLength = rulerModel.Point1.DistanceTo(rulerModel.Point2);
                        }
                        else
                        {
                            rulerModel.Point2 = new Point3D(rulerModel.Point1.X, rulerModel.Point1.Y, rulerModel.Point1.Z);
                            StraightedgeLength = 0;
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        private void AddStartPointStraightedgeAction()
        {
            if (_viewPort == null || _viewPort.Children == null) return;

            foreach (var child in _viewPort.Children)
            {
                switch (child)
                {
                    /*
                    case StraightedgeModel rulerModel:
                        if (_viewPort.CursorOnElementPosition.HasValue)
                        {
                            rulerModel.AddStartPoint(_viewPort.CursorOnElementPosition.Value);
                            StraightedgeLength = rulerModel.Length;
                        }
                        else
                        {
                            rulerModel.AddStartPoint(Point.Origin());
                            StraightedgeLength = rulerModel.Length;
                        }
                        break;

                    case StraightedgeNotchModel rulerNotchModel:
                        if (_viewPort.CursorOnElementPosition.HasValue)
                        {
                            rulerNotchModel.AddStartPoint(_viewPort.CursorOnElementPosition.Value);
                        }
                        else
                        {
                            rulerNotchModel.AddStartPoint(Point.Origin());
                        }
                        break;
                    */
                    case ArrowVisual3D rulerModel:
                        if (_viewPort.CursorOnElementPosition.HasValue)
                        {
                            rulerModel.Point1 = _viewPort.CursorOnElementPosition.Value;
                            StraightedgeLength = 0;
                        }
                        else
                        {
                            rulerModel.Point1 = new Point3D(0,0,0);
                            StraightedgeLength = 0;
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        private void DeleteRuler()
        {
            if (_viewPort == null || _viewPort.Children == null) return;

            _viewPort.Children
                .Where(e => e is RulerModel || e is RulerNotchModel)
                .ToList()
                .ForEach(e => _viewPort.Children.Remove(e));
        }

        private void DeleteStraightedge()
        {
            if (_viewPort == null || _viewPort.Children == null) return;
            /*
            _viewPort.Children
                .Where(e => e is StraightedgeModel || e is StraightedgeNotchModel)
                .ToList()
                .ForEach(e => _viewPort.Children.Remove(e));
            */
            _viewPort.Children
                .Where(e => e is ArrowVisual3D )
                .ToList()
                .ForEach(e => _viewPort.Children.Remove(e));
        }
        #endregion RULER
    }
}
