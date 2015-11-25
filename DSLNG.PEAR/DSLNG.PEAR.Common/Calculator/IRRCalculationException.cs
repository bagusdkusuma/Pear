using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Common.Calculator
{
    public class IRRCalculationException : Exception
    {
        public IRRCalculationException(string message)
            : base(message)
        {
        }
    }
}
