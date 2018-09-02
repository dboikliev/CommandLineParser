using System;

namespace CommandLineParser.Exceptions
{
    public class MissingValueForOptionException : Exception
    {
        public MissingValueForOptionException(string command, string optionArgumentName) 
            : base($"{command}: An option with name \"{optionArgumentName}\" was provided, but it is missing a value.")
        {

        }
    }
}