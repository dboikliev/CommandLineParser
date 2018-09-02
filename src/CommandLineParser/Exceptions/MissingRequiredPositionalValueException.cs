using System;
namespace CommandLineParser.Exceptions
{
    public class MissingRequiredPositionalValueException : Exception
    {
        public MissingRequiredPositionalValueException(string command, string name)
            : base($"{command}: The positional value \"{name}\" is required.")
        {
            
        }
    }
}
