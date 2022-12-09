using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginFramework.Installation
{
    public class FileToRemoveContainer : IEnumerable<string>
    {
        private readonly List<string> _files = new List<string>();
        public void RemoveAll()
        {
            FileHelper.RemoveFiles(_files);
        }
        public void Add(string file) => _files.Add(file);
        public void AddRange(List<string> files) => _files.AddRange(files);

        public IEnumerator<string> GetEnumerator() => _files.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
