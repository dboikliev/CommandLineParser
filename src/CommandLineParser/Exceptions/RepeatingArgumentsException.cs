using System;

namespace CommandLineParser.Exceptions
{
    public class RepeatingArgumentsException : Exception
    {
        public RepeatingArgumentsException(string message) : base(message)
        {
            
        }
    }
}
