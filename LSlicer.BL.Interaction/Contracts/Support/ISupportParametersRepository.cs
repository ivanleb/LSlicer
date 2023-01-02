using LSlicer.BL.Interaction.Contracts;
using LSlicer.Data.Interaction.Contracts;
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
