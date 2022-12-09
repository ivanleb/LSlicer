using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.BL.Interaction.Contracts
{
    public interface IParametersService<T>
    {
        IEnumerable<T> GetAll();
        void Set(T parameters, int partId, FileInfo fileInfo);
        IList<string> GetParametersIdentifiers(int partId);
        void Save(IEnumerable<T> parameters);
        IList<FileInfo> TakeOutParameters(int parametersId);
        void Add(T parameters);
        void Delete(T parameters);
    }
}
