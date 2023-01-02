namespace LSlicer.Data.Interaction.Contracts
{
    public interface ISlicingParameters: IIdentifier
    {
        double Thickness { get; set; }
    }

    public static class SlicingParametersExtentions 
    {
        public static string GetIdentifier(this ISlicingParameters parameters) 
        {
            return parameters.Id + ":" + parameters.Thickness;
        }
    }
}
