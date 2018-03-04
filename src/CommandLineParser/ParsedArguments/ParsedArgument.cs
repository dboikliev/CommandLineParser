using System;
using System.Collections.Generic;

namespace CommandLineParser.ParsedArguments
{
    public class ParsedArgument
    {
        public string CommandName { get; set; }
        public Type Type { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Values { get; set; } = new List<string>();
    }
}
