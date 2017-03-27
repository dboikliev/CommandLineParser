using System;

namespace CommandLineParser.Exceptions
{
    public class HelpException : Exception
    {
        public string CommandName { get; }

        public HelpException(string commandName)
        {
            CommandName = commandName;
        }
    }
}