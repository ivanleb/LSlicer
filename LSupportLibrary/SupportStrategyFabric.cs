using LSlicer.Data.Interaction;
using LSlicer.Data.Model;
using LSlicer.Data.Interaction.Contracts;
using System.Threading;

namespace LSupportLibrary
{
    public static class SupportStrategyFabric
    {
        public static IMakeSupportStrategy Create(IPart part, ISupportParameters supportParameters, int numberFrom, CancellationToken token) 
        {
            if (part.IsSupport()) 
                return new EmptySupportStrategy();

            switch (supportParameters.Type)
            {
                case SupportType.Body:

                    return new BodySupportStrategy(supportParameters, numberFrom, token);

                case SupportType.Cross:

                    return new CrossSupportStrategy(supportParameters, numberFrom, token);

                case SupportType.Grid:

                    return new GridSupportStrategy(supportParameters, numberFrom, token);

                default:

                    return new EmptySupportStrategy();
            }
        }
    }
}
