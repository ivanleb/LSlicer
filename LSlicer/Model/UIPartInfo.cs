using LSlicer.Data.Interaction;
using LSlicer.Data.Model;
using LSlicer.Data.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LSlicer.Model
{
    public class UIPartInfo
    {
        private IPart _part;
        private Func<int, string> _getSlicingPathDelegate;
        public UIPartInfo(IPart part)
        {
            _part = part;
            SupportParametersInfos = new ObservableCollection<string>();
            SlicingParametersInfos = new ObservableCollection<string>();
            _getSlicingPathDelegate = i => ""; 
        }

        public UIPartInfo(IPart part, IList<string> supportParameters, IList<string> slicingParameters, Func<int, string> getSlicingPathDelegate) 
        {
            _part = part;
            SupportParametersInfos = new ObservableCollection<string>(supportParameters);
            SlicingParametersInfos = new ObservableCollection<string>(slicingParameters);
            _getSlicingPathDelegate = getSlicingPathDelegate;
        }

        public string Name { get => _part.PartSpec.MeshFilePath; }
        public string Id { get => _part.Id.ToString(); }
        public string SliceFileInfo
        {
            get
            {
                string info = _getSlicingPathDelegate?.Invoke(_part.Id); 

                return String.IsNullOrEmpty(info) ? "" : info;
            }
        }

        public ObservableCollection<string> SupportParametersInfos { get; }

        public ObservableCollection<string> SlicingParametersInfos { get; }

        public static implicit operator UIPartInfo(Part part)
        {
            return new UIPartInfo(part);
        }
    }
}
