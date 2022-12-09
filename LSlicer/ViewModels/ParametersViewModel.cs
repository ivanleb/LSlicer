using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LSlicer.ViewModels
{
    public class ParametersViewModel : BindableBase
    {

        public ParametersViewModel()
        {
            ExecuteSetSlicingVis();
        }

        private Visibility _slicingVisibility;
        public Visibility SlicingVisibility
        {
            get { return _slicingVisibility; }
            set { SetProperty(ref _slicingVisibility, value); }
        }

        private Visibility _supportVisibility;
        public Visibility SupportVisibility
        {
            get { return _supportVisibility; }
            set { SetProperty(ref _supportVisibility, value); }
        }

        private DelegateCommand _setSlicingVis;
        public DelegateCommand SetSlicingVis =>
            _setSlicingVis ?? (_setSlicingVis = new DelegateCommand(ExecuteSetSlicingVis));

        void ExecuteSetSlicingVis()
        {
            SlicingVisibility = Visibility.Visible;
            SupportVisibility = Visibility.Collapsed;
        }

        private DelegateCommand _setSupportVis;

        public DelegateCommand SetSupportVis =>
            _setSupportVis ?? (_setSupportVis = new DelegateCommand(ExecuteSetSupportVis));

        void ExecuteSetSupportVis()
        {
            SlicingVisibility = Visibility.Collapsed;
            SupportVisibility = Visibility.Visible;
        }
    }
}
