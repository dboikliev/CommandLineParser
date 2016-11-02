using System;

namespace CommandLineParser
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FlagAttribute : ArgumentAttribute
    {
        public FlagAttribute(char shortName, string longName) : base(shortName, longName)
        {
        }
    }
}