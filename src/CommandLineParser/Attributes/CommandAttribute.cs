using System;

namespace CommandLineParser.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CommandAttribute : ArgumentAttribute
    {
        public string Name { get; }

        public CommandAttribute(string name)
        {
            Name = name;
        }
    }
}
