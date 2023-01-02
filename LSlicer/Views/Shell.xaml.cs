using HelixToolkit.Wpf;
using LSlicer.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace LSlicer.Views
{
    public partial class Shell : Window
    {
        private bool _isTranslateShow;
        private bool _isRotateShow;
        private bool _isScaleShow;

        public Shell()
        {
            InitializeComponent();
            //viewPort3d.RotateGesture = new MouseGesture(MouseAction.RightClick);
            viewPort3d.Children.Add(new ModelVisual3D());
            viewPort3d.Children.Add(new SunLight());

            //viewPort3d.Children.Add(new GridLinesVisual3D() { Width = 100, Length = 100, MinorDistance = 10, MajorDistance = 10, Thickness = 0.1, Fill = GradientBrushes.HueStripes, Center = new Point3D(0,0,0.1) });
            SphereVisual3D sphere = new SphereVisual3D();
            viewPort3d.Children.Add(sphere);
            viewPort3d.DefaultCamera = new PerspectiveCamera(new Point3D(-20, -40, 20), new Vector3D(17, 34, -17), new Vector3D(0, 0, 1), 0.9);
            viewPort3d.Drop += ViewPort3d_Drop;
            if (DataContext is ShellViewModel dataContext)
            {
                dataContext.SetViewPortCommands(viewPort3d);
            }

        }

        private void ViewPort3d_Drop(object sender, DragEventArgs e)
        {
            if (DataContext is ShellViewModel dataContext)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                    foreach (var file in files)
                    {
                        dataContext.LoadModelAction(file);
                    }
                }
            }
        }

        private void ShowRightMenuTranslateButton_Click(object sender, RoutedEventArgs e)
        {
            menuslide("ShowRightMenu", HideRightMenuTranslateButton, ShowRightMenuTranslateButton, RightMenuTranslatePanel);
            _isTranslateShow = true;
            if (DataContext is ShellViewModel viewModel)
            {
                //viewModel.NeedAddManipulator = true;
                viewModel.UnSetSelectionMouseBindings();
            }
        }

        private void HideRightMenuTranslateButton_Click(object sender, RoutedEventArgs e)
        {
            menuslide("HideRightMenu", HideRightMenuTranslateButton, ShowRightMenuTranslateButton, RightMenuTranslatePanel);
            _isTranslateShow = false;
            if (_isRotateShow || _isScaleShow) return;

            if (DataContext is ShellViewModel viewModel)
            {
                //viewModel.NeedAddManipulator = false;
                viewModel.SetSelectionMouseBindings();
            }
        }

        private void ShowRightMenuRotateButton_Click(object sender, RoutedEventArgs e)
        {
            menuslide("ShowRightMenu", HideRightMenuRotateButton, ShowRightMenuRotateButton, RightMenuRotatePanel);
            _isRotateShow = true;
            if (DataContext is ShellViewModel viewModel)
            {
                //viewModel.NeedAddManipulator = true;
                viewModel.UnSetSelectionMouseBindings();
            }
        }

        private void HideRightMenuRotateButton_Click(object sender, RoutedEventArgs e)
        {
            menuslide("HideRightMenu", HideRightMenuRotateButton, ShowRightMenuRotateButton, RightMenuRotatePanel);
            _isRotateShow = false;

            if (_isTranslateShow || _isScaleShow) return;

            if (DataContext is ShellViewModel viewModel)
            {
                //viewModel.NeedAddManipulator = false;
                viewModel.SetSelectionMouseBindings();
            }
        }

        private void ShowRightMenuScaleButton_Click(object sender, RoutedEventArgs e)
        {
            menuslide("ShowRightMenu", HideRightMenuScaleButton, ShowRightMenuScaleButton, RightMenuScalePanel);
            _isScaleShow = true;
            if (DataContext is ShellViewModel viewModel)
            {
                //viewModel.NeedAddManipulator = true;
                viewModel.UnSetSelectionMouseBindings();
            }
        }

        private void HideRightMenuScaleButton_Click(object sender, RoutedEventArgs e)
        {
            menuslide("HideRightMenu", HideRightMenuScaleButton, ShowRightMenuScaleButton, RightMenuScalePanel);
            _isScaleShow = false;
            if (_isRotateShow || _isTranslateShow) return;

            if (DataContext is ShellViewModel viewModel)
            {
                //viewModel.NeedAddManipulator = false;
                viewModel.SetSelectionMouseBindings();
            }

        }

        private void menuslide(string p, Button btnhide, Button btnshow, StackPanel menu)
        {
            Storyboard sb = Resources[p] as Storyboard;

            sb.Begin(menu);
            if (p.Contains("Show"))
            {
                btnhide.Visibility = System.Windows.Visibility.Visible;
                btnshow.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                if (p.Contains("Hide"))
                {
                    btnshow.Visibility = System.Windows.Visibility.Visible;
                    btnhide.Visibility = System.Windows.Visibility.Hidden;
                }
            }
        }

        private void RotateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isRotateShow)
            {
                HideRightMenuRotateButton_Click(sender, e);
            }
            else
            {
                ShowRightMenuRotateButton_Click(sender, e);
            }
        }

        private void TranslateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isTranslateShow)
            {
                HideRightMenuTranslateButton_Click(sender, e);
            }
            else
            {
                ShowRightMenuTranslateButton_Click(sender, e);
            }
        }

        private void ScaleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isScaleShow)
            {
                HideRightMenuScaleButton_Click(sender, e);
            }
            else
            {
                ShowRightMenuScaleButton_Click(sender, e);
            }
        }

        private void CopyPartContextMenu_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ShellViewModel dataContext)
            {
                if (dataContext.CopyPartCommand.CanExecute())
                    dataContext.CopyPartCommand.Execute();
            }
        }

        private void RemovePartContextMenu_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ShellViewModel dataContext)
            {
                if (dataContext.RemovePartCommand.CanExecute())
                    dataContext.RemovePartCommand.Execute();
            }
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is ShellViewModel dataContext)
            {
                if (dataContext.UpdateRulerCommand.CanExecute())
                    dataContext.UpdateRulerCommand.Execute();

                if (dataContext.UpdateStraightedgeCommand.CanExecute())
                    dataContext.UpdateStraightedgeCommand.Execute();
            }
        }

        private void viewPort3d_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (DataContext is ShellViewModel dataContext)
            {
                if (dataContext.AddStartPointStraightedgeCommand.CanExecute())
                    dataContext.AddStartPointStraightedgeCommand.Execute();
            }

        }

        private void TextBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}
