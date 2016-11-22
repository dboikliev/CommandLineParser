using System;

namespace CommandLineParser.Exceptions
{
    public class MissingValueForOptionException : Exception
    {
        public MissingValueForOptionException(string optionArgumentName) 
            : base($"An option with name {optionArgumentName} was provided, but it is missing a value.")
        {

        }
    }
}