using System.Collections.Generic;
using CommandLineParser.ParsedArguments;

namespace CommandLineParser.TypeParsers
{
    internal interface ITypedEnumerableParser<TParsed> : ITypedParser
    {
        new IEnumerable<TParsed> Parse(ParsedArgument argument);
    }
}