using System;
using System.Linq;
using CommandLineParser.ParsedArguments;

namespace CommandLineParser.TypeParsers
{
    public class StringParser : BaseValueParser<string>
    {
        public override string Parse(ParsedArgument argument)
        {
            return argument.Values.FirstOrDefault();
        }
    }
}
