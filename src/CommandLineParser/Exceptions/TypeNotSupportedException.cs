using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineParser.Exceptions
{
    public class TypeNotSupportedException : Exception
    {
        public TypeNotSupportedException(Type type) : base($"The type { type } is not supported.")
        {

        }
    }
}
