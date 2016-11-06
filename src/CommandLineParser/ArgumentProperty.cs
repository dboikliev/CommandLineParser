using System;
using System.Collections.Generic;
using System.Reflection;

namespace CommandLineParser
{
    public class ArgumentProperty
    {
        public PropertyInfo Property { get; set; }
        public ArgumentAttribute Argument { get; set; }
        public List<string> Values { get; set; } = new List<string>();
        public Type Type { get; set; }
    }
}