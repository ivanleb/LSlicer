using LSlicer.Data.Interaction;

namespace LSlicer.Data.Model
{
    public class SlicingInfo : ISlicingInfo
    {
        public int Count { get;set; }  
        public double Thiсkness { get; set; }
        public string FilePath { get; set; }
        public string Name { get; set; }
        public int PartId { get; set; }
        public OperationType Type => OperationType.Slicing;
    }
}
