using System.Reflection;

namespace CommandLineParser
{
    public class ArgumentProperty
    {
        public PropertyInfo Property { get; set; }
        public ArgumentAttribute Argument { get; set; }
    }
}