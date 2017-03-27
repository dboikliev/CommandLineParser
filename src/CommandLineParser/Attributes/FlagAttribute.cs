using System;

namespace CommandLineParser.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class FlagAttribute : ArgumentAttribute
    {
        public string ShortName { get; }
        public string LongName { get; }

        public FlagAttribute(char shortName, string longName)
        {
            ShortName = shortName.ToString();
            LongName = longName;
        }
    }
}
