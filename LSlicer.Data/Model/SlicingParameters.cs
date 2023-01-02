using LSlicer.Data.Interaction.Contracts;

namespace LSlicer.Data.Model
{
    public class SlicingParameters : ISlicingParameters
    {
        public int Id { get; set; }
        public double Thickness { get; set; }
    }
}
