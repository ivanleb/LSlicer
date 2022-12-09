using LSlicer.Data.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace LSlicer.Data.Model
{
    public class EmptyPart : IPart
    {
        public IPartSpec PartSpec => null;

        public IList<IOperation> Operations => new List<IOperation>();

        public IPartTransform ResultTransform => new PartTransform("Init", Matrix3D.Identity, Matrix3D.Identity);

        private int _id = Int32.MinValue;
        public int Id { get => _id; set => _id = value; }

        public int LinkToParentPart { get; set; }

        public IPart Copy(int newId) => new EmptyPart();

        public void Transform(IPartTransform transform)
        {
            //todo: трансформ для всех деталей
        }
    }
}
