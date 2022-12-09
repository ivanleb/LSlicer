using LSlicer.BL.Interaction.Contracts;
using LSlicing.Data.Interaction.Contracts;
using System.Collections.Generic;

namespace LSlicer.BL.Interaction
{
    public interface ISupportParametersRepository : IRepository<ISupportParameters>
    {
        IEnumerable<ISupportParameters> GetAll();
        /// <summary>
        /// возвращает последний(наибольший) id
        /// </summary>
        /// <returns></returns>
        int LastId();
    }
}
