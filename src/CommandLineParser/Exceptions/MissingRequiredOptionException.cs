using System;

namespace CommandLineParser.Exceptions
{
    public class MissingRequiredOptionException : Exception
    {
        public MissingRequiredOptionException(string optionShortName, string optionLongName) 
            : base($"The option \"{optionShortName}|{optionLongName}\" is required.")
        {
        }
    }
}
