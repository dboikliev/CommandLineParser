using System;

namespace CommandLineParser.Exceptions
{
    public class MissingRequiredOptionException : Exception
    {
        public MissingRequiredOptionException(string command, string optionShortName, string optionLongName) 
            : base($"{command}: The option \"{optionShortName}|{optionLongName}\" is required.")
        {
        }
    }
}
