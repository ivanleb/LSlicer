using System.Collections.Generic;
using System.Linq;

namespace LSlicer.Helpers 
{
    public class CmdLine
    {
        IList<string> _args;
        public IList<string> Args { get => _args; }
        public CmdLine(IList<string> args)
        {
            _args = args;
        }

        public string GetProcessPath()
        {
            return Args.First();
        }

        public string GetArgs()
        {
            return _args.Skip(1).Aggregate((s1, s2) => s1 + " " + s2);
        }
    }
}
