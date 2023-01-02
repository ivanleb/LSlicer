using LSlicer.Data.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.BL.Domain
{
    public static class SlicingParametersValidator
    {
        private const double maxThickness = 5;
        private const double minThickness = 0.005;

        public static bool Check(ISlicingParameters slicingParameters)
        {
            return (slicingParameters.Thickness <= maxThickness 
                && slicingParameters.Thickness >= minThickness);
        }
    }
}
