using LSlicer.BL.Interaction.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.BL.Interaction
{
    public interface IGeneratorHive<T>
    {
        T Get(string engineName);
    }
}
