using LSlicer.Data.Interaction;
using LSlicer.Data.Interaction.Contracts;
using LSlicingLibrary.SliceStrategies;
using System;

namespace LSlicingLibrary
{
    public static class SliceStrategyFabric
    {
        public static ISliceStrategy Create(IPart part, ISlicingParameters slicingParameters) 
        {
            switch (part.PartSpec.PartType)
            {
                case PartType.Volume:
                    return new PartSliceStrategy(slicingParameters);
                case PartType.Support:
                    return new SupportSliceStrategy(slicingParameters);
                default:
                    throw new NotSupportedException($"{part.PartSpec.PartType} is not supported");
            }
        }
    }
}
