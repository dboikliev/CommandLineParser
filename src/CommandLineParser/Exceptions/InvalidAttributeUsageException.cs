using System;

namespace CommandLineParser.Exceptions
{
    public class InvalidAttributeUsageException : Exception
    {
        public InvalidAttributeUsageException(string message) : base(message)
        {
            
        }
    }
}
