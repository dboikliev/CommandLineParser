using System;
namespace CommandLineParser.Exceptions
{
    public class MissingRequiredPositionalValueException : Exception
    {
        public MissingRequiredPositionalValueException(string name)
            : base($"The positional value {name} is required.")
        {
            
        }
    }
}
