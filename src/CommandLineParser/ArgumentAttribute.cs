using System;

namespace CommandLineParser
{
    public abstract class ArgumentAttribute : Attribute
    {
        public string ShortName { get; }
        public string LongName { get;  }
        public bool IsRequired { get; set; } = false;

        protected ArgumentAttribute(string shortName, string longName)
        {
            ShortName = shortName;
            LongName = longName;
        }
    }
}
