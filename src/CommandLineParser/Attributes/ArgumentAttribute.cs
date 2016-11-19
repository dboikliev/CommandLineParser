using System;

namespace CommandLineParser.Attributes
{
    public abstract class ArgumentAttribute : Attribute
    {
        public string Description { get; set; }
    }
}
