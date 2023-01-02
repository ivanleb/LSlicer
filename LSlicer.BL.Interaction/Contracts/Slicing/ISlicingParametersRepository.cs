using LSlicer.Data.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.BL.Interaction.Contracts
{
    public interface ISlicingParametersRepository : IRepository<ISlicingParameters>
    {
        IEnumerable<ISlicingParameters> GetAll();
        /// <summary>
        /// возвращает последний(наибольший) Id
        /// </summary>
        /// <returns></returns>
        int LastId();
    }
}
