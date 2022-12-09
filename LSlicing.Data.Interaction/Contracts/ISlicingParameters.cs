using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicing.Data.Interaction.Contracts
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
