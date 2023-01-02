using GeometRi;
using HelixToolkit.Wpf;
using EngineHelpers;
using LSlicer.Data.Interaction;
using LSlicer.Data.Model;
using LSlicer.Data.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace LSlicingLibrary.SliceStrategies
{
    public abstract class BaseSliceStrategy : ISliceStrategy
    {
        protected IList<IList<IList<Segment>>> _resultContour = new List<IList<IList<Segment>>>();
        protected ModelVisual3D _modelVisual3D = new ModelVisual3D();
        protected MeshGeometry3D _meshGeometry;
        protected double _thickness = 0.02;

        protected BaseSliceStrategy(ISlicingParameters slicingParameters)
        {
            _thickness = slicingParameters.Thickness;
        }

        public void ApplyTransform(IPartTransform partTransform)
        {
            Mesh.ApplyTransform(partTransform, _modelVisual3D, _meshGeometry);
        }

        public void Load(IPart part)
        {
            (_modelVisual3D, _meshGeometry) = Mesh.Load(part.PartSpec.MeshFilePath);
        }

        public abstract ISlicingInfo Slice(IPart inPart, string slicingPath);

        public abstract void WriteToFile(string FilePath);

    }
}
