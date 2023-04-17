using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginFramework.CustomPlugin.Zipping
{
    public interface IZipFileFilter
    {
        bool Filter(string fileName);
    }
}
