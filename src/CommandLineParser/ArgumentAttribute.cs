using System;

namespace CommandLineParser
{
    public abstract class ArgumentAttribute : Attribute
    {
        public char ShortName { get; }
        public string LongName { get;  }
        public bool IsRequired { get; set; } = false;

        protected ArgumentAttribute()
        {
        }

        protected ArgumentAttribute(char shortName, string longName)
        {
            ShortName = shortName;
            LongName = longName;
        }
    }
}
