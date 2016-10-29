using System;

namespace CommandLineParser
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionAttribute : ArgumentAttribute
    {
        public OptionAttribute(string shortName, string longName) : base(shortName, longName)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : ArgumentAttribute
    {
        public CommandAttribute(string shortName, string longName) : base(shortName, longName)
        {
        }
    }
}
