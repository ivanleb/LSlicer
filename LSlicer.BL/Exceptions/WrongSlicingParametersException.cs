using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSlicer.BL.Exceptions
{
    public class WrongSlicingParametersException : Exception
    {
        public WrongSlicingParametersException(string message) : base(message)
        {
        }

        public override string Message => "Wrong slicing parameters:" + base.Message ;
    }
}
