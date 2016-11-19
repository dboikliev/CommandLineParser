using System;

namespace CommandLineParser.Exceptions
{
    public class MissingValueForRequiredOption : Exception
    {
        public MissingValueForRequiredOption(string optionArgumentName) : base($"A value is missing for required option with name {optionArgumentName}")
        {

        }
    }
}