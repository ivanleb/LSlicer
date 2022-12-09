using System;
using System.Collections.Generic;
using LSlicer.Data.Interaction;

namespace LSlicer.BL.Interaction.Contracts
{
    public interface ISliceService
    {
        void MakeSlicing(IList<IPart> parts);
    }
}