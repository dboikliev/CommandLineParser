using System.Reflection;
using CommandLineParser.Attributes;

namespace CommandLineParser
{
    public class ArgumentProperty
    {
        public ArgumentAttribute Argument { get; set; }
        public PropertyInfo Property { get; set; }
        public bool Evaluated { get; set; }
    }
}
