using System;

namespace CommandLineParser
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : ArgumentAttribute
    {
        public CommandAttribute(string shortName, string longName) : base(shortName, longName)
        {
        }
    }
}