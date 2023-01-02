using LSlicer.Data.Interaction;
using LSlicer.Data.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace LSlicer.Data.Model
{
    public class Part : IPart
    {
        private readonly IPartSpec _partSpec;

        public Part(IPartSpec partSpec)
        {
            _partSpec = partSpec;
            ResultTransform = new PartTransform("Result transform", Matrix3D.Identity, Matrix3D.Identity);
        }

        public IPartSpec PartSpec => _partSpec;

        public IPartTransform ResultTransform { get; set; }
        public int Id { get => PartSpec.Id; set => PartSpec.Id = value; }

        public int LinkToParentPart { get; set; }

        public void Transform(IPartTransform transform)
        {
            var transformMatrix = transform.RelativeMatrix;

            Matrix3D newTransformMatrix = Matrix3D.Multiply(ResultTransform.RelativeMatrix, transformMatrix);

            ResultTransform.RelativeMatrix = newTransformMatrix;

            ResultTransform = new PartTransform(ResultTransform.Name, ResultTransform.RelativeMatrix, ResultTransform.AbsoluteMatrix);

        }

        public IPart Copy(int newId)
        {
            var spec = new PartSpec(newId, _partSpec);
            return new Part(spec) { ResultTransform = ResultTransform };
        }
    }
}
