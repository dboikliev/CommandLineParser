using System;

namespace CommandLineParser.Exceptions
{
    public class TypeNotSupportedException : Exception
    {
        public TypeNotSupportedException(Type type) : base($"The type { type } is not supported.")
        {

        }
    }
}
