using LSlicer.Data.Interaction;
using Newtonsoft.Json;

namespace LSlicer.Data.Model
{
    public class PartDataForSave : IPartDataForSave
    {
        public PartDataForSave(IPart part, IOperationInfo[] operations)
        {
            Spec = part.PartSpec;
            LinkToParentPart = part.LinkToParentPart;
            Operations = operations;
        }

        [JsonConstructor]
        public PartDataForSave(){}

        [JsonConverter(typeof(ConcreteTypeConverter<PartSpec>))]
        public IPartSpec Spec { get; set; }

        public IOperationInfo[] Operations { get; set; }

        public int LinkToParentPart { get; set; }
    }
}
