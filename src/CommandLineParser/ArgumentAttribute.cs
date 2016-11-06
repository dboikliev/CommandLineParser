using System;

namespace CommandLineParser
{
    public abstract class ArgumentAttribute : Attribute
    {
        public bool IsRequired { get; set; } = false;
        public string Description { get; set; }
    }
}
