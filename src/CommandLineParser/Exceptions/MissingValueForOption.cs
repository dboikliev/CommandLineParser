using System;

namespace CommandLineParser.Exceptions
{
    public class MissingValueForOption : Exception
    {
        public MissingValueForOption(string optionArgumentName) : base($"An option with name {optionArgumentName} was provided, but it is missing a value.")
        {

        }
    }
}